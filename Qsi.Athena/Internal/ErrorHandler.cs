using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;

namespace Qsi.Athena.Internal;

internal sealed class ErrorHandler : BaseErrorListener
{
    private readonly ISet<int> _ignoredRules;
    private readonly IDictionary<int, string> _specialRules;
    private readonly IDictionary<int, string> _specialTokens;

    private ErrorHandler(IDictionary<int, string> specialRules, IDictionary<int, string> specialTokens, ISet<int> ignoredRules)
    {
        _specialRules = specialRules;
        _specialTokens = specialTokens;
        _ignoredRules = ignoredRules;
    }

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        try
        {
            var parser = (Parser)recognizer;
            var atn = parser.Atn;

            ATNState currentState;
            IToken currentToken;
            RuleContext context;

            if (e != null)
            {
                currentState = atn.states[e.OffendingState];
                currentToken = e.OffendingToken;
                context = e.Context;

                if (e is NoViableAltException noViableAltException)
                    currentToken = noViableAltException.StartToken;
            }
            else
            {
                currentState = atn.states[parser.State];
                currentToken = parser.CurrentToken;
                context = parser.Context;
            }

            var analyzer = new Analyzer(parser, _specialRules, _specialTokens, _ignoredRules);
            var result = analyzer.Process(currentState, currentToken.TokenIndex, context);

            // pick the candidate tokens associated largest token index processed (i.e., the path that consumed the most input)
            var expected = string.Join(", ", result.Expected);

            msg = $"mismatched input '{parser.TokenStream.Get(result.ErrorTokenIndex).Text}'. Expecting: {expected}";
        }
        catch (Exception)
        {
            // LOG.log(SEVERE, "Unexpected failure when handling parsing error. This is likely a bug in the implementation", exception);
        }

        throw new ParsingException(msg, line, charPositionInLine + 1);
    }

    public static Builder CreateBuilder()
    {
        return new Builder();
    }

    private sealed class ParsingState
    {
        public readonly Parser _parser;
        public readonly ATNState _state;
        public readonly bool _suppressed;
        public readonly int _tokenIndex;

        public ParsingState(ATNState state, int tokenIndex, bool suppressed, Parser parser)
        {
            _state = state;
            _tokenIndex = tokenIndex;
            _suppressed = suppressed;
            _parser = parser;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ParsingState that)
                return false;

            return _tokenIndex == that._tokenIndex &&
                   _state.Equals(that._state);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_state, _tokenIndex);
        }

        public override string ToString()
        {
            var token = _parser.TokenStream.Get(_tokenIndex);
            var text = string.IsNullOrEmpty(token.Text) ? "?" : token.Text;

            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace("\\", "\\\\");
                text = text.Replace("\n", "\\n");
                text = text.Replace("\r", "\\r");
                text = text.Replace("\t", "\\t");
            }

            // "%s%s:%s @ %s:<%s>:%s"
            var builder = new StringBuilder();

            builder
                .Append(_suppressed ? '-' : '+')
                .Append(_parser.RuleNames[_state.ruleIndex])
                .Append(':')
                .Append(_state.stateNumber);

            builder.Append(" @ ");

            builder
                .Append(_tokenIndex)
                .Append(":<")
                .Append(_parser.Vocabulary.GetSymbolicName(token.Type))
                .Append(">:")
                .Append(text);

            return builder.ToString();
        }
    }

    private class Analyzer
    {
        private readonly ATN _atn;

        private readonly ISet<string> _candidates = new HashSet<string>();
        private readonly ISet<int> _ignoredRules;
        private readonly IDictionary<ParsingState, ISet<int>> _memo = new Dictionary<ParsingState, ISet<int>>();
        private readonly Parser _parser;
        private readonly IDictionary<int, string> _specialRules;
        private readonly IDictionary<int, string> _specialTokens;
        private readonly ITokenStream _stream;
        private readonly IVocabulary _vocabulary;

        private int furthestTokenIndex = -1;

        public Analyzer(
            Parser parser,
            IDictionary<int, string> specialRules,
            IDictionary<int, string> specialTokens,
            ISet<int> ignoredRules)
        {
            _parser = parser;
            _stream = parser.TokenStream;
            _atn = parser.Atn;
            _vocabulary = parser.Vocabulary;
            _specialRules = specialRules;
            _specialTokens = specialTokens;
            _ignoredRules = ignoredRules;
        }

        public Result Process(ATNState currentState, int tokenIndex, RuleContext context)
        {
            var startState = _atn.ruleToStartState[currentState.ruleIndex];

            if (IsReachable(currentState, startState))
                // We've been dropped inside a rule in a state that's reachable via epsilon transitions. This is,
                // effectively, equivalent to starting at the beginning (or immediately outside) the rule.
                // In that case, backtrack to the beginning to be able to take advantage of logic that remaps
                // some rules to well-known names for reporting purposes
                currentState = startState;

            ISet<int> endTokens = Process(new ParsingState(currentState, tokenIndex, false, _parser), 0);
            ISet<int> nextTokens = new HashSet<int>();

            while (endTokens.Count > 0 && context.invokingState != -1)
            {
                foreach (var endToken in endTokens)
                {
                    var nextState = ((RuleTransition)_atn.states[context.invokingState].Transition(0)).followState;

                    foreach (var token in Process(new ParsingState(nextState, endToken, false, _parser), 0))
                        nextTokens.Add(token);
                }

                context = context.Parent;
                endTokens = nextTokens;
            }

            return new Result(furthestTokenIndex, _candidates);
        }

        private bool IsReachable(ATNState target, RuleStartState from)
        {
            var activeStates = new Queue<ATNState>();
            activeStates.Enqueue(from);

            while (activeStates.TryDequeue(out var current))
            {
                if (current.stateNumber == target.stateNumber)
                    return true;

                for (var i = 0; i < current.NumberOfTransitions; i++)
                {
                    var transition = current.Transition(i);

                    if (transition.IsEpsilon)
                        activeStates.Enqueue(transition.target);
                }
            }

            return false;
        }

        private ISet<int> Process(ParsingState start, int precedence)
        {
            if (_memo.TryGetValue(start, out ISet<int> result))
                return result;

            var endTokens = new HashSet<int>();

            // Simulates the ATN by consuming input tokens and walking transitions.
            // The ATN can be in multiple states (similar to an NFA)
            var activeStates = new Queue<ParsingState>();
            activeStates.Enqueue(start);

            while (activeStates.TryDequeue(out var current))
            {
                var state = current._state;
                var tokenIndex = current._tokenIndex;
                var suppressed = current._suppressed;

                while (_stream.Get(tokenIndex).Channel == Lexer.Hidden)
                    // Ignore whitespace
                    tokenIndex++;

                var currentToken = _stream.Get(tokenIndex).Type;

                if (state.StateType == StateType.RuleStart)
                {
                    var rule = state.ruleIndex;

                    if (_specialRules.TryGetValue(rule, out var specialRule))
                    {
                        if (!suppressed)
                            Record(tokenIndex, specialRule);

                        suppressed = true;
                    }
                    else if (_ignoredRules.Contains(rule))
                    {
                        // TODO expand ignored rules like we expand special rules
                        continue;
                    }
                }

                if (state is RuleStopState)
                {
                    endTokens.Add(tokenIndex);
                    continue;
                }

                for (var i = 0; i < state.NumberOfTransitions; i++)
                {
                    var transition = state.Transition(i);

                    switch (transition)
                    {
                        case RuleTransition ruleTransition:
                        {
                            var parsingState = new ParsingState(ruleTransition.target, tokenIndex, suppressed, _parser);

                            foreach (var endToken in Process(parsingState, ruleTransition.precedence))
                                activeStates.Enqueue(new ParsingState(ruleTransition.followState, endToken, suppressed, _parser));

                            break;
                        }

                        case PrecedencePredicateTransition predicateTransition:
                        {
                            if (precedence < predicateTransition.precedence)
                                activeStates.Enqueue(new ParsingState(predicateTransition.target, tokenIndex, suppressed, _parser));

                            break;
                        }

                        default:
                        {
                            if (transition.IsEpsilon)
                            {
                                activeStates.Enqueue(new ParsingState(transition.target, tokenIndex, suppressed, _parser));
                            }
                            else if (transition is WildcardTransition)
                            {
                                throw new NotSupportedException("not yet implemented: wildcard transition");
                            }
                            else
                            {
                                var labels = transition.Label;

                                if (transition is NotSetTransition)
                                    labels = labels.Complement(IntervalSet.Of(TokenConstants.MinUserTokenType, _atn.maxTokenType));

                                // Surprisingly, TokenStream (i.e. BufferedTokenStream) may not have loaded all the tokens from the
                                // underlying stream. TokenStream.get() does not force tokens to be buffered -- it just returns what's
                                // in the current buffer, or fail with an IndexOutOfBoundsError. Since Antlr decided the error occurred
                                // within the current set of buffered tokens, stop when we reach the end of the buffer.
                                if (labels.Contains(currentToken) && tokenIndex < _stream.Size - 1)
                                {
                                    activeStates.Enqueue(new ParsingState(transition.target, tokenIndex + 1, false, _parser));
                                }
                                else
                                {
                                    if (!suppressed)
                                        Record(tokenIndex, GetTokenNames(labels));
                                }
                            }

                            break;
                        }
                    }
                }
            }

            result = endTokens;
            _memo[start] = result;

            return result;
        }

        private void Record(int tokenIndex, string label)
        {
            Record(tokenIndex, new[] { label });
        }

        private void Record(int tokenIndex, IEnumerable<string> labels)
        {
            if (tokenIndex >= furthestTokenIndex)
            {
                if (tokenIndex > furthestTokenIndex)
                {
                    _candidates.Clear();
                    furthestTokenIndex = tokenIndex;
                }

                foreach (var label in labels)
                    _candidates.Add(label);
            }
        }

        private IEnumerable<string> GetTokenNames(IntervalSet tokens)
        {
            var names = new HashSet<string>();

            for (var i = 0; i < tokens.Count; i++)
            {
                var token = GetToken(tokens, i);

                if (token == TokenConstants.EOF)
                {
                    names.Add("<EOF>");
                }
                else
                {
                    if (!_specialTokens.TryGetValue(token, out var specialToken))
                        specialToken = _vocabulary.GetDisplayName(token);

                    names.Add(specialToken);
                }
            }

            return names;
        }

        private int GetToken(IntervalSet tokens, int i)
        {
            IList<Interval> intervals = tokens.GetIntervals();
            var index = 0;

            foreach (var interval in intervals)
            {
                var a = interval.a;
                var b = interval.b;

                for (var v = a; v <= b; ++v)
                {
                    if (index == i)
                        return v;

                    ++index;
                }
            }

            return -1;
        }
    }

    public sealed class Builder
    {
        private readonly ISet<int> _ignoredRules = new HashSet<int>();
        private readonly IDictionary<int, string> _specialRules = new Dictionary<int, string>();
        private readonly IDictionary<int, string> _specialTokens = new Dictionary<int, string>();

        public Builder SpecialRule(int ruleId, string name)
        {
            _specialRules[ruleId] = name;
            return this;
        }

        public Builder SpecialToken(int tokenId, string name)
        {
            _specialTokens[tokenId] = name;
            return this;
        }

        public Builder IgnoredRule(int ruleId)
        {
            _ignoredRules.Add(ruleId);
            return this;
        }

        public ErrorHandler Build()
        {
            return new ErrorHandler(_specialRules, _specialTokens, _ignoredRules);
        }
    }

    private sealed class Result
    {
        public Result(int errorTokenIndex, ISet<string> expected)
        {
            ErrorTokenIndex = errorTokenIndex;
            Expected = expected;
        }

        public int ErrorTokenIndex { get; }

        public ISet<string> Expected { get; }
    }
}
