using System.Collections.Generic;
using Qsi.Data;

namespace Qsi.Analyzers.Action.Cache;

/// <summary>
/// 사용된 서브쿼리를 분석한 Table Structure 리스트를 캐싱하는 인터페이스입니다.
/// </summary>
public interface ITableStructureCache<in T>
{
    /// <summary>
    /// T에 해당하는 Table Structure를 추가합니다.
    /// </summary>
    /// <param name="key">캐시를 저장할 키 값입니다.</param>
    /// <param name="table">캐싱할 테이블입니다.</param>
    void Add(T key, QsiTableStructure table);

    /// <summary>
    /// T에 해당하는 Table Structure 목록을 추가합니다.
    /// </summary>
    /// <param name="key">캐시를 저장할 키 값입니다.</param>
    /// <param name="tables">캐싱할 테이블 목록입니다.</param>
    void AddRange(T key, IEnumerable<QsiTableStructure> tables);

    /// <summary>
    /// T에 해당하는 Table Structure 목록을 가져옵니다.
    /// </summary>
    /// <param name="key">목록을 식별할 수 있는 키 값입니다.</param>
    /// <returns>Table Structure 목록입니다.</returns>
    ICollection<QsiTableStructure> Get(T key);

    /// <summary>
    /// 캐시를 초기화합니다.
    /// </summary>
    void Clear();
}
