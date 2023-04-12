--
-- Data for Name: actor; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.actor (actor_id, first_name, last_name, last_update) FROM stdin;
\.
COPY public.actor (actor_id, first_name, last_name, last_update) FROM '~/Downloads/dvdrental/3057.dat';

--
-- Data for Name: address; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.address (address_id, address, address2, district, city_id, postal_code, phone, last_update) FROM stdin;
\.
COPY public.address (address_id, address, address2, district, city_id, postal_code, phone, last_update) FROM '~/Downloads/dvdrental/3065.dat';

--
-- Data for Name: category; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.category (category_id, name, last_update) FROM stdin;
\.
COPY public.category (category_id, name, last_update) FROM '~/Downloads/dvdrental/3059.dat';

--
-- Data for Name: city; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.city (city_id, city, country_id, last_update) FROM stdin;
\.
COPY public.city (city_id, city, country_id, last_update) FROM '~/Downloads/dvdrental/3067.dat';

--
-- Data for Name: country; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.country (country_id, country, last_update) FROM stdin;
\.
COPY public.country (country_id, country, last_update) FROM '~/Downloads/dvdrental/3069.dat';

--
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.customer (customer_id, store_id, first_name, last_name, email, address_id, activebool, create_date, last_update, active) FROM stdin;
\.
COPY public.customer (customer_id, store_id, first_name, last_name, email, address_id, activebool, create_date, last_update, active) FROM '~/Downloads/dvdrental/3055.dat';

--
-- Data for Name: film; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.film (film_id, title, description, release_year, language_id, rental_duration, rental_rate, length, replacement_cost, rating, last_update, special_features, fulltext) FROM stdin;
\.
COPY public.film (film_id, title, description, release_year, language_id, rental_duration, rental_rate, length, replacement_cost, rating, last_update, special_features, fulltext) FROM '~/Downloads/dvdrental/3061.dat';

--
-- Data for Name: film_actor; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.film_actor (actor_id, film_id, last_update) FROM stdin;
\.
COPY public.film_actor (actor_id, film_id, last_update) FROM '~/Downloads/dvdrental/3062.dat';

--
-- Data for Name: film_category; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.film_category (film_id, category_id, last_update) FROM stdin;
\.
COPY public.film_category (film_id, category_id, last_update) FROM '~/Downloads/dvdrental/3063.dat';

--
-- Data for Name: inventory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.inventory (inventory_id, film_id, store_id, last_update) FROM stdin;
\.
COPY public.inventory (inventory_id, film_id, store_id, last_update) FROM '~/Downloads/dvdrental/3071.dat';

--
-- Data for Name: language; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.language (language_id, name, last_update) FROM stdin;
\.
COPY public.language (language_id, name, last_update) FROM '~/Downloads/dvdrental/3073.dat';

--
-- Data for Name: payment; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.payment (payment_id, customer_id, staff_id, rental_id, amount, payment_date) FROM stdin;
\.
COPY public.payment (payment_id, customer_id, staff_id, rental_id, amount, payment_date) FROM '~/Downloads/dvdrental/3075.dat';

--
-- Data for Name: rental; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rental (rental_id, rental_date, inventory_id, customer_id, return_date, staff_id, last_update) FROM stdin;
\.
COPY public.rental (rental_id, rental_date, inventory_id, customer_id, return_date, staff_id, last_update) FROM '~/Downloads/dvdrental/3077.dat';

--
-- Data for Name: staff; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.staff (staff_id, first_name, last_name, address_id, email, store_id, active, username, password, last_update, picture) FROM stdin;
\.
COPY public.staff (staff_id, first_name, last_name, address_id, email, store_id, active, username, password, last_update, picture) FROM '~/Downloads/dvdrental/3079.dat';

--
-- Data for Name: store; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.store (store_id, manager_staff_id, address_id, last_update) FROM stdin;
\.
COPY public.store (store_id, manager_staff_id, address_id, last_update) FROM '~/Downloads/dvdrental/3081.dat';


Name: actor_actor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
