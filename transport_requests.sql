--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2
-- Dumped by pg_dump version 17.2

-- Started on 2026-03-20 10:55:06

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 115446)
-- Name: Applications; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."Applications" (
    "Id" integer NOT NULL,
    "Number" character varying(50),
    "Status" character varying(50) NOT NULL,
    "ApplicationDate" timestamp with time zone NOT NULL,
    "OrganizationUnit" character varying(100) NOT NULL,
    "ResponsiblePerson" character varying(100) NOT NULL,
    "Phone" character varying(20) NOT NULL,
    "Purpose" character varying(200) NOT NULL,
    "Route" character varying(500) NOT NULL,
    "Notes" character varying(1000),
    "DispatcherName" character varying(100),
    "DispatcherPhone" character varying(20),
    "DriverName" character varying(100),
    "DriverPhone" character varying(20),
    "VehicleBrand" character varying(50),
    "VehicleNumber" character varying(20),
    "VehicleColor" character varying(30),
    "DispatcherNotes" character varying(1000),
    "CreatedAt" timestamp with time zone DEFAULT now() NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "TripEnd" timestamp with time zone,
    "TripStart" timestamp with time zone,
    "Passengers" character varying(20) DEFAULT ''::character varying
);


ALTER TABLE public."Applications" OWNER TO transport_user;

--
-- TOC entry 4899 (class 0 OID 0)
-- Dependencies: 219
-- Name: TABLE "Applications"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON TABLE public."Applications" IS 'Таблица транспортных заявок';


--
-- TOC entry 4900 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Id"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Id" IS 'Уникальный идентификатор заявки';


--
-- TOC entry 4901 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Number"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Number" IS 'Номер заявки (формат: ГГГГММДД-XXXX)';


--
-- TOC entry 4902 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Status"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Status" IS 'Статус заявки (CreatedOrModified, Approved, InProgress, Completed, RejectedByDispatcher, RejectedByDirector, AssignedToVehicle, NotCompleted)';


--
-- TOC entry 4903 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."ApplicationDate"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."ApplicationDate" IS 'Дата подачи заявки';


--
-- TOC entry 4904 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."OrganizationUnit"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."OrganizationUnit" IS 'Подразделение заказчика';


--
-- TOC entry 4905 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."ResponsiblePerson"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."ResponsiblePerson" IS 'ФИО ответственного лица';


--
-- TOC entry 4906 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Phone"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Phone" IS 'Контактный телефон';


--
-- TOC entry 4907 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Purpose"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Purpose" IS 'Цель поездки';


--
-- TOC entry 4908 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Route"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Route" IS 'Маршрут следования';


--
-- TOC entry 4909 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Notes"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Notes" IS 'Примечания заказчика';


--
-- TOC entry 4910 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."DispatcherName"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."DispatcherName" IS 'ФИО диспетчера';


--
-- TOC entry 4911 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."DispatcherPhone"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."DispatcherPhone" IS 'Телефон диспетчера';


--
-- TOC entry 4912 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."DriverName"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."DriverName" IS 'ФИО водителя';


--
-- TOC entry 4913 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."DriverPhone"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."DriverPhone" IS 'Телефон водителя';


--
-- TOC entry 4914 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."VehicleBrand"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."VehicleBrand" IS 'Марка транспортного средства';


--
-- TOC entry 4915 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."VehicleNumber"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."VehicleNumber" IS 'Государственный номер ТС';


--
-- TOC entry 4916 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."VehicleColor"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."VehicleColor" IS 'Цвет ТС';


--
-- TOC entry 4917 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."DispatcherNotes"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."DispatcherNotes" IS 'Примечания диспетчера';


--
-- TOC entry 4918 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."CreatedAt"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."CreatedAt" IS 'Дата и время создания записи';


--
-- TOC entry 4919 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."UpdatedAt"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."UpdatedAt" IS 'Дата и время последнего обновления';


--
-- TOC entry 4920 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."TripEnd"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."TripEnd" IS 'Дата и время окончания поездки';


--
-- TOC entry 4921 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."TripStart"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."TripStart" IS 'Дата и время начала поездки';


--
-- TOC entry 4922 (class 0 OID 0)
-- Dependencies: 219
-- Name: COLUMN "Applications"."Passengers"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."Applications"."Passengers" IS 'Список пассажиров';


--
-- TOC entry 231 (class 1259 OID 131973)
-- Name: ApplicationFullInfo; Type: VIEW; Schema: public; Owner: transport_user
--

CREATE VIEW public."ApplicationFullInfo" AS
 SELECT "Id",
    "Number",
    "Status",
        CASE "Status"
            WHEN 'Approved'::text THEN 'Утверждена'::text
            WHEN 'InProgress'::text THEN 'Исполняется'::text
            WHEN 'Completed'::text THEN 'Исполнена'::text
            WHEN 'CreatedOrModified'::text THEN 'Создана/Изменена'::text
            WHEN 'RejectedByDispatcher'::text THEN 'Отклонена диспетчером'::text
            WHEN 'RejectedByDirector'::text THEN 'Отклонена руководителем'::text
            WHEN 'AssignedToVehicle'::text THEN 'Назначено ТС'::text
            WHEN 'NotCompleted'::text THEN 'Не исполнена'::text
            ELSE 'Неизвестно'::text
        END AS "StatusName",
    "ApplicationDate",
    "OrganizationUnit",
    "ResponsiblePerson",
    "Phone",
    "Purpose",
    "Route",
    "Passengers",
    "Notes",
    "DispatcherName",
    "DispatcherPhone",
    "DriverName",
    "DriverPhone",
    "VehicleBrand",
    "VehicleNumber",
    "VehicleColor",
    "DispatcherNotes",
    "TripStart",
    "TripEnd",
    "CreatedAt",
    "UpdatedAt"
   FROM public."Applications" a;


ALTER VIEW public."ApplicationFullInfo" OWNER TO transport_user;

--
-- TOC entry 4923 (class 0 OID 0)
-- Dependencies: 231
-- Name: VIEW "ApplicationFullInfo"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON VIEW public."ApplicationFullInfo" IS 'Полная информация о заявках с расшифровкой статуса';


--
-- TOC entry 218 (class 1259 OID 115445)
-- Name: Applications_Id_seq; Type: SEQUENCE; Schema: public; Owner: transport_user
--

ALTER TABLE public."Applications" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Applications_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 225 (class 1259 OID 115483)
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO transport_user;

--
-- TOC entry 224 (class 1259 OID 115482)
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: transport_user
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 220 (class 1259 OID 115454)
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO transport_user;

--
-- TOC entry 227 (class 1259 OID 115496)
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO transport_user;

--
-- TOC entry 226 (class 1259 OID 115495)
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: transport_user
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 228 (class 1259 OID 115508)
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" character varying(128) NOT NULL,
    "ProviderKey" character varying(128) NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO transport_user;

--
-- TOC entry 229 (class 1259 OID 115520)
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO transport_user;

--
-- TOC entry 230 (class 1259 OID 115537)
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" character varying(128) NOT NULL,
    "Name" character varying(128) NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO transport_user;

--
-- TOC entry 221 (class 1259 OID 115461)
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO transport_user;

--
-- TOC entry 223 (class 1259 OID 115469)
-- Name: StatusHistory; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."StatusHistory" (
    "Id" integer NOT NULL,
    "ApplicationId" integer NOT NULL,
    "OldStatus" character varying(50) NOT NULL,
    "NewStatus" character varying(50) NOT NULL,
    "ChangedBy" character varying(100) NOT NULL,
    "Comment" character varying(500),
    "ChangedDate" timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public."StatusHistory" OWNER TO transport_user;

--
-- TOC entry 4924 (class 0 OID 0)
-- Dependencies: 223
-- Name: TABLE "StatusHistory"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON TABLE public."StatusHistory" IS 'История изменений статусов заявок';


--
-- TOC entry 4925 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."Id"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."Id" IS 'Уникальный идентификатор записи истории';


--
-- TOC entry 4926 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."ApplicationId"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."ApplicationId" IS 'ID заявки (внешний ключ к таблице Applications)';


--
-- TOC entry 4927 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."OldStatus"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."OldStatus" IS 'Предыдущий статус заявки';


--
-- TOC entry 4928 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."NewStatus"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."NewStatus" IS 'Новый статус заявки';


--
-- TOC entry 4929 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."ChangedBy"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."ChangedBy" IS 'Кто изменил статус';


--
-- TOC entry 4930 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."Comment"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."Comment" IS 'Комментарий к изменению статуса';


--
-- TOC entry 4931 (class 0 OID 0)
-- Dependencies: 223
-- Name: COLUMN "StatusHistory"."ChangedDate"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON COLUMN public."StatusHistory"."ChangedDate" IS 'Дата и время изменения статуса';


--
-- TOC entry 222 (class 1259 OID 115468)
-- Name: StatusHistory_Id_seq; Type: SEQUENCE; Schema: public; Owner: transport_user
--

ALTER TABLE public."StatusHistory" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."StatusHistory_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 232 (class 1259 OID 131978)
-- Name: StatusStatistics; Type: VIEW; Schema: public; Owner: transport_user
--

CREATE VIEW public."StatusStatistics" AS
 SELECT "Status",
    count(*) AS "Count",
        CASE "Status"
            WHEN 'Approved'::text THEN 'Утверждена'::text
            WHEN 'InProgress'::text THEN 'Исполняется'::text
            WHEN 'Completed'::text THEN 'Исполнена'::text
            WHEN 'CreatedOrModified'::text THEN 'Создана/Изменена'::text
            WHEN 'RejectedByDispatcher'::text THEN 'Отклонена диспетчером'::text
            WHEN 'RejectedByDirector'::text THEN 'Отклонена руководителем'::text
            WHEN 'AssignedToVehicle'::text THEN 'Назначено ТС'::text
            WHEN 'NotCompleted'::text THEN 'Не исполнена'::text
            ELSE NULL::text
        END AS "StatusName"
   FROM public."Applications"
  GROUP BY "Status"
  ORDER BY (count(*)) DESC;


ALTER VIEW public."StatusStatistics" OWNER TO transport_user;

--
-- TOC entry 4932 (class 0 OID 0)
-- Dependencies: 232
-- Name: VIEW "StatusStatistics"; Type: COMMENT; Schema: public; Owner: transport_user
--

COMMENT ON VIEW public."StatusStatistics" IS 'Статистика распределения заявок по статусам';


--
-- TOC entry 217 (class 1259 OID 115440)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: transport_user
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO transport_user;

--
-- TOC entry 4881 (class 0 OID 115446)
-- Dependencies: 219
-- Data for Name: Applications; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."Applications" ("Id", "Number", "Status", "ApplicationDate", "OrganizationUnit", "ResponsiblePerson", "Phone", "Purpose", "Route", "Notes", "DispatcherName", "DispatcherPhone", "DriverName", "DriverPhone", "VehicleBrand", "VehicleNumber", "VehicleColor", "DispatcherNotes", "CreatedAt", "UpdatedAt", "TripEnd", "TripStart", "Passengers") FROM stdin;
31	20260303-5394	CreatedOrModified	2026-03-14 00:00:00+05	аааааа	ппппппппп	ппппппппп	пппп	пппппппппппппппппп	ппппппп	аааааааа	аааааааа	а	\N	\N	\N	\N	\N	2026-03-03 14:10:46.51731+05	\N	2026-03-19 19:10:00+05	2026-03-20 19:10:00+05	ппппппппп
32	20260317-5841	CreatedOrModified	2026-03-12 00:00:00+05	Тест	Тест	123	fffffffffffff	уу	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-17 07:17:09.790625+05	\N	2026-03-13 12:16:00+05	2026-03-19 12:16:00+05	\N
33	20260317-6693	CreatedOrModified	2026-03-14 00:00:00+05	ааа	аа	а	а	а	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-17 12:24:36.608378+05	\N	2026-03-13 12:24:00+05	2026-03-10 12:24:00+05	\N
34	20260317-7829	CreatedOrModified	2026-03-20 00:00:00+05	уууууууу	нн	нн	нн	нн	нн	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-17 12:31:48.799831+05	\N	\N	\N	нн
35	20260317-6283	CreatedOrModified	2026-03-14 00:00:00+05	ккк	кк	ккк	кк	кк	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-17 12:32:21.839362+05	\N	\N	2026-03-27 12:32:00+05	кк
36	20260317-5578	CreatedOrModified	2026-03-13 00:00:00+05	аааааа	ааааааааааа	ккк	ааааааааааааа	вв	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-17 12:38:15.832678+05	\N	\N	2026-03-13 12:38:00+05	\N
2	20260301-1002	CreatedOrModified	2026-03-01 00:00:00+05	Бухгалтерия	Петрова Анна Сергеевна	+7(495)234-56-78	Сдача отчетности	Офис - Налоговая - Офис	Важно!	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-02 21:29:57.389874+05	\N	2026-03-03 15:30:00+05	2026-03-03 10:30:00+05	1
3	20260228-2001	NotCompleted	2026-02-28 00:00:00+05	Отдел продаж	Сидоров Петр Петрович	+7(495)345-67-89	Командировка в филиал	Москва - Нижний Новгород - Москва	Заказ гостиницы	Алексеева Елена	+7(495)456-78-90	Смирнов Дмитрий	+7(495)567-89-01	Toyota Camry	А123ВВ777	Белый	Встретить на вокзале	2026-03-02 21:29:57.389874+05	\N	2026-03-06 20:00:00+05	2026-03-04 08:00:00+05	3
4	20260228-2002	NotCompleted	2026-02-28 00:00:00+05	HR отдел	Соколова Мария Ивановна	+7(495)678-90-12	Собеседование с кандидатами	Офис - Бизнес-центр - Офис	\N	Козлов Андрей	+7(495)789-01-23	Васильев Игорь	+7(495)890-12-34	Kia Rio	В456СД777	Серебристый	Подать машину к 13:45	2026-03-02 21:29:57.389874+05	\N	2026-03-05 16:00:00+05	2026-03-05 14:00:00+05	2
5	20260227-3001	RejectedByDirector	2026-02-27 00:00:00+05	Логистика	Николаев Алексей	+7(495)901-23-45	Доставка документов	Склад - Офис - Банк	Срочно	Морозова Татьяна	+7(495)012-34-56	Федоров Павел	+7(495)123-45-67	Lada Largus	Е789ОР777	Синий	\N	2026-03-02 21:29:57.389874+05	\N	2026-03-02 14:00:00+05	2026-03-02 11:00:00+05	1
6	20260227-3002	RejectedByDirector	2026-02-27 00:00:00+05	ИТ-отдел	Владимиров Константин	+7(495)234-56-78	Обслуживание серверов	Офис - Дата-центр - Офис	Пропуск заказан	Григорьев Антон	+7(495)345-67-89	Степанов Роман	+7(495)456-78-90	Ford Transit	К012АВ777	Белый	Грузоподъемность до 1 тонны	2026-03-02 21:29:57.389874+05	\N	2026-03-03 17:30:00+05	2026-03-03 09:30:00+05	2
7	20260225-4001	RejectedByDispatcher	2026-02-25 00:00:00+05	Склад	Александров Сергей	+7(495)567-89-01	Перевозка груза	Склад №1 - Склад №2	Хрупкий груз	Дмитриева Ольга	+7(495)678-90-12	Павлов Андрей	+7(495)789-01-23	Газель NEXT	Т345УЕ777	Белый	Отметка о выполнении: груз доставлен	2026-03-02 21:29:57.389874+05	\N	2026-02-25 12:00:00+05	2026-02-25 10:00:00+05	2
1	20260301-1001	Approved	2026-03-01 00:00:00+05	Администрация	Иванов Иван Иванович	+7(495)123-45-67	Деловая встреча	Москва - Санкт-Петербург - Москва	Срочная поездка	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-02 21:29:57.389874+05	2026-03-19 15:01:27.815759+05	2026-03-02 18:00:00+05	2026-03-02 09:00:00+05	2
8	20260224-4002	RejectedByDispatcher	2026-02-24 00:00:00+05	Маркетинг	Викторова Елена	+7(495)890-12-34	Забор рекламных материалов	Типография - Офис	\N	Зайцев Максим	+7(495)901-23-45	Соловьев Денис	+7(495)012-34-56	Hyundai Solaris	У567РЕ777	Черный	Забрать 10 коробок	2026-03-02 21:29:57.389874+05	\N	2026-02-24 17:00:00+05	2026-02-24 15:00:00+05	1
9	20260226-5001	InProgress	2026-02-26 00:00:00+05	Производство	Кузнецов Андрей	+7(495)123-45-67	Поездка на завод	Офис - Завод - Офис	\N	Семенова Ирина	+7(495)234-56-78	\N	\N	\N	\N	\N	Отказ: нет свободных водителей	2026-03-02 21:29:57.389874+05	\N	2026-02-27 18:00:00+05	2026-02-27 09:00:00+05	4
10	20260226-5002	Approved	2026-02-26 00:00:00+05	Юридический отдел	Лебедева Наталья	+7(495)345-67-89	Срочная командировка	Москва - Тула - Москва	Срочно!	Ковалева Анна	+7(495)456-78-90	\N	\N	\N	\N	\N	Отказ руководителя: нет согласования	2026-03-02 21:29:57.389874+05	\N	2026-02-28 20:00:00+05	2026-02-28 08:00:00+05	1
11	20260301-6001	Completed	2026-03-01 00:00:00+05	Дирекция	Михайлов Илья	+7(495)567-89-01	Поездка на конференцию	Москва - Казань - Москва	VIP-обслуживание	Никитина Светлана	+7(495)678-90-12	Орлов Виктор	+7(495)789-01-23	Mercedes-Benz V-Class	О777АА777	Черный	Машина подана, водитель ожидает	2026-03-02 21:29:57.389874+05	\N	2026-03-06 22:00:00+05	2026-03-06 07:00:00+05	3
12	20260220-7001	AssignedToVehicle	2026-02-20 00:00:00+05	Техподдержка	Громов Денис	+7(495)890-12-34	Выезд к клиенту	Офис - Клиент - Офис	Срочный ремонт	Белов Александр	+7(495)901-23-45	Волков Сергей	+7(495)012-34-56	Renault Logan	А789ВС777	Серый	Не выполнено: клиент отменил встречу	2026-03-02 21:29:57.389874+05	\N	2026-02-21 18:00:00+05	2026-02-21 09:00:00+05	2
13	20260215-8001	CreatedOrModified	2026-02-15 00:00:00+05	Архив	Федоров Петр	+7(495)123-45-67	Сдача документов в архив	Офис - Архив	Старая заявка	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-02 21:29:57.389874+05	\N	2026-02-16 16:00:00+05	2026-02-16 10:00:00+05	1
14	20260210-8002	RejectedByDispatcher	2026-02-10 00:00:00+05	Курьерская служба	Егоров Максим	+7(495)234-56-78	Доставка корреспонденции	Почта - Офис	Исполнено	Васильева Екатерина	+7(495)345-67-89	Андреев Андрей	+7(495)456-78-90	Lada Granta	В123КЕ777	Бежевый	Заявка закрыта	2026-03-02 21:29:57.389874+05	\N	2026-02-10 13:00:00+05	2026-02-10 11:00:00+05	1
15	20260205-8003	NotCompleted	2026-02-05 00:00:00+05	Плановый отдел	Николаева Татьяна	+7(495)567-89-01	Планерка в главном офисе	Офис - Главный офис	Утверждено	Сергеев Дмитрий	+7(495)678-90-12	Иванов Иван	+7(495)789-01-23	Volkswagen Caravelle	К789АА777	Серебристый	Все готово	2026-03-02 21:29:57.389874+05	\N	2026-02-06 18:00:00+05	2026-02-06 09:00:00+05	5
16	20260301-1001	CreatedOrModified	2026-03-01 00:00:00+05	Администрация	Иванов Иван Иванович	+7(495)123-45-67	Деловая встреча	Москва - Санкт-Петербург - Москва	Срочная поездка	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-02 21:30:04.092777+05	\N	2026-03-02 18:00:00+05	2026-03-02 09:00:00+05	2
17	20260301-1002	CreatedOrModified	2026-03-01 00:00:00+05	Бухгалтерия	Петрова Анна Сергеевна	+7(495)234-56-78	Сдача отчетности	Офис - Налоговая - Офис	Важно!	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-02 21:30:04.092777+05	\N	2026-03-03 15:30:00+05	2026-03-03 10:30:00+05	1
18	20260228-2001	NotCompleted	2026-02-28 00:00:00+05	Отдел продаж	Сидоров Петр Петрович	+7(495)345-67-89	Командировка в филиал	Москва - Нижний Новгород - Москва	Заказ гостиницы	Алексеева Елена	+7(495)456-78-90	Смирнов Дмитрий	+7(495)567-89-01	Toyota Camry	А123ВВ777	Белый	Встретить на вокзале	2026-03-02 21:30:04.092777+05	\N	2026-03-06 20:00:00+05	2026-03-04 08:00:00+05	3
19	20260228-2002	NotCompleted	2026-02-28 00:00:00+05	HR отдел	Соколова Мария Ивановна	+7(495)678-90-12	Собеседование с кандидатами	Офис - Бизнес-центр - Офис	\N	Козлов Андрей	+7(495)789-01-23	Васильев Игорь	+7(495)890-12-34	Kia Rio	В456СД777	Серебристый	Подать машину к 13:45	2026-03-02 21:30:04.092777+05	\N	2026-03-05 16:00:00+05	2026-03-05 14:00:00+05	2
20	20260227-3001	RejectedByDirector	2026-02-27 00:00:00+05	Логистика	Николаев Алексей	+7(495)901-23-45	Доставка документов	Склад - Офис - Банк	Срочно	Морозова Татьяна	+7(495)012-34-56	Федоров Павел	+7(495)123-45-67	Lada Largus	Е789ОР777	Синий	\N	2026-03-02 21:30:04.092777+05	\N	2026-03-02 14:00:00+05	2026-03-02 11:00:00+05	1
21	20260227-3002	RejectedByDirector	2026-02-27 00:00:00+05	ИТ-отдел	Владимиров Константин	+7(495)234-56-78	Обслуживание серверов	Офис - Дата-центр - Офис	Пропуск заказан	Григорьев Антон	+7(495)345-67-89	Степанов Роман	+7(495)456-78-90	Ford Transit	К012АВ777	Белый	Грузоподъемность до 1 тонны	2026-03-02 21:30:04.092777+05	\N	2026-03-03 17:30:00+05	2026-03-03 09:30:00+05	2
22	20260225-4001	RejectedByDispatcher	2026-02-25 00:00:00+05	Склад	Александров Сергей	+7(495)567-89-01	Перевозка груза	Склад №1 - Склад №2	Хрупкий груз	Дмитриева Ольга	+7(495)678-90-12	Павлов Андрей	+7(495)789-01-23	Газель NEXT	Т345УЕ777	Белый	Отметка о выполнении: груз доставлен	2026-03-02 21:30:04.092777+05	\N	2026-02-25 12:00:00+05	2026-02-25 10:00:00+05	2
23	20260224-4002	RejectedByDispatcher	2026-02-24 00:00:00+05	Маркетинг	Викторова Елена	+7(495)890-12-34	Забор рекламных материалов	Типография - Офис	\N	Зайцев Максим	+7(495)901-23-45	Соловьев Денис	+7(495)012-34-56	Hyundai Solaris	У567РЕ777	Черный	Забрать 10 коробок	2026-03-02 21:30:04.092777+05	\N	2026-02-24 17:00:00+05	2026-02-24 15:00:00+05	1
24	20260226-5001	InProgress	2026-02-26 00:00:00+05	Производство	Кузнецов Андрей	+7(495)123-45-67	Поездка на завод	Офис - Завод - Офис	\N	Семенова Ирина	+7(495)234-56-78	\N	\N	\N	\N	\N	Отказ: нет свободных водителей	2026-03-02 21:30:04.092777+05	\N	2026-02-27 18:00:00+05	2026-02-27 09:00:00+05	4
25	20260226-5002	Approved	2026-02-26 00:00:00+05	Юридический отдел	Лебедева Наталья	+7(495)345-67-89	Срочная командировка	Москва - Тула - Москва	Срочно!	Ковалева Анна	+7(495)456-78-90	\N	\N	\N	\N	\N	Отказ руководителя: нет согласования	2026-03-02 21:30:04.092777+05	\N	2026-02-28 20:00:00+05	2026-02-28 08:00:00+05	1
26	20260301-6001	Completed	2026-03-01 00:00:00+05	Дирекция	Михайлов Илья	+7(495)567-89-01	Поездка на конференцию	Москва - Казань - Москва	VIP-обслуживание	Никитина Светлана	+7(495)678-90-12	Орлов Виктор	+7(495)789-01-23	Mercedes-Benz V-Class	О777АА777	Черный	Машина подана, водитель ожидает	2026-03-02 21:30:04.092777+05	\N	2026-03-06 22:00:00+05	2026-03-06 07:00:00+05	3
27	20260220-7001	AssignedToVehicle	2026-02-20 00:00:00+05	Техподдержка	Громов Денис	+7(495)890-12-34	Выезд к клиенту	Офис - Клиент - Офис	Срочный ремонт	Белов Александр	+7(495)901-23-45	Волков Сергей	+7(495)012-34-56	Renault Logan	А789ВС777	Серый	Не выполнено: клиент отменил встречу	2026-03-02 21:30:04.092777+05	\N	2026-02-21 18:00:00+05	2026-02-21 09:00:00+05	2
28	20260215-8001	CreatedOrModified	2026-02-15 00:00:00+05	Архив	Федоров Петр	+7(495)123-45-67	Сдача документов в архив	Офис - Архив	Старая заявка	\N	\N	\N	\N	\N	\N	\N	\N	2026-03-02 21:30:04.092777+05	\N	2026-02-16 16:00:00+05	2026-02-16 10:00:00+05	1
29	20260210-8002	RejectedByDispatcher	2026-02-10 00:00:00+05	Курьерская служба	Егоров Максим	+7(495)234-56-78	Доставка корреспонденции	Почта - Офис	Исполнено	Васильева Екатерина	+7(495)345-67-89	Андреев Андрей	+7(495)456-78-90	Lada Granta	В123КЕ777	Бежевый	Заявка закрыта	2026-03-02 21:30:04.092777+05	\N	2026-02-10 13:00:00+05	2026-02-10 11:00:00+05	1
30	20260205-8003	NotCompleted	2026-02-05 00:00:00+05	Плановый отдел	Николаева Татьяна	+7(495)567-89-01	Планерка в главном офисе	Офис - Главный офис	Утверждено	Сергеев Дмитрий	+7(495)678-90-12	Иванов Иван	+7(495)789-01-23	Volkswagen Caravelle	К789АА777	Серебристый	Все готово	2026-03-02 21:30:04.092777+05	\N	2026-02-06 18:00:00+05	2026-02-06 09:00:00+05	5
\.


--
-- TOC entry 4887 (class 0 OID 115483)
-- Dependencies: 225
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- TOC entry 4882 (class 0 OID 115454)
-- Dependencies: 220
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
\.


--
-- TOC entry 4889 (class 0 OID 115496)
-- Dependencies: 227
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetUserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- TOC entry 4890 (class 0 OID 115508)
-- Dependencies: 228
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetUserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
\.


--
-- TOC entry 4891 (class 0 OID 115520)
-- Dependencies: 229
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetUserRoles" ("UserId", "RoleId") FROM stdin;
\.


--
-- TOC entry 4892 (class 0 OID 115537)
-- Dependencies: 230
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetUserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- TOC entry 4883 (class 0 OID 115461)
-- Dependencies: 221
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
\.


--
-- TOC entry 4885 (class 0 OID 115469)
-- Dependencies: 223
-- Data for Name: StatusHistory; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."StatusHistory" ("Id", "ApplicationId", "OldStatus", "NewStatus", "ChangedBy", "Comment", "ChangedDate") FROM stdin;
1	1	4	8	Иванов И.И.	\N	2026-03-01 10:30:00+05
2	3	4	3	Петров П.П.	\N	2026-02-28 11:00:00+05
3	3	3	6	Сидоров С.С.	\N	2026-03-02 08:30:00+05
4	9	4	1	Директор	\N	2026-02-26 16:45:00+05
5	11	4	3	Алексеева Е.	\N	2026-03-01 09:15:00+05
6	6	4	8	Руководитель	\N	2026-02-25 11:30:00+05
7	6	8	5	Водитель	\N	2026-02-25 13:00:00+05
8	1	4	8	Иванов И.И.	\N	2026-03-01 10:30:00+05
9	3	4	3	Петров П.П.	\N	2026-02-28 11:00:00+05
10	3	3	6	Сидоров С.С.	\N	2026-03-02 08:30:00+05
11	9	4	1	Директор	\N	2026-02-26 16:45:00+05
12	11	4	3	Алексеева Е.	\N	2026-03-01 09:15:00+05
13	6	4	8	Руководитель	\N	2026-02-25 11:30:00+05
14	6	8	5	Водитель	\N	2026-02-25 13:00:00+05
21	1	CreatedOrModified	Approved	System	\N	2026-03-19 20:01:27.817633+05
\.


--
-- TOC entry 4879 (class 0 OID 115440)
-- Dependencies: 217
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: transport_user
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260121151035_InitialCreate	8.0.22
20260213175003_AddTripStartEnd	8.0.22
20260213175255_AddTripStartEndFields	8.0.22
20260223194056_AddTripFieldsToConfig	8.0.22
20260226172727_AddPassengersCount	8.0.22
\.


--
-- TOC entry 4933 (class 0 OID 0)
-- Dependencies: 218
-- Name: Applications_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: transport_user
--

SELECT pg_catalog.setval('public."Applications_Id_seq"', 36, true);


--
-- TOC entry 4934 (class 0 OID 0)
-- Dependencies: 224
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: transport_user
--

SELECT pg_catalog.setval('public."AspNetRoleClaims_Id_seq"', 1, false);


--
-- TOC entry 4935 (class 0 OID 0)
-- Dependencies: 226
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: transport_user
--

SELECT pg_catalog.setval('public."AspNetUserClaims_Id_seq"', 1, false);


--
-- TOC entry 4936 (class 0 OID 0)
-- Dependencies: 222
-- Name: StatusHistory_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: transport_user
--

SELECT pg_catalog.setval('public."StatusHistory_Id_seq"', 21, true);


--
-- TOC entry 4698 (class 2606 OID 115453)
-- Name: Applications PK_Applications; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."Applications"
    ADD CONSTRAINT "PK_Applications" PRIMARY KEY ("Id");


--
-- TOC entry 4712 (class 2606 OID 115489)
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- TOC entry 4700 (class 2606 OID 115460)
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- TOC entry 4715 (class 2606 OID 115502)
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- TOC entry 4718 (class 2606 OID 115514)
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- TOC entry 4721 (class 2606 OID 115526)
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- TOC entry 4723 (class 2606 OID 115543)
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- TOC entry 4704 (class 2606 OID 115467)
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- TOC entry 4709 (class 2606 OID 115476)
-- Name: StatusHistory PK_StatusHistory; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."StatusHistory"
    ADD CONSTRAINT "PK_StatusHistory" PRIMARY KEY ("Id");


--
-- TOC entry 4692 (class 2606 OID 115444)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 4702 (class 1259 OID 115554)
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- TOC entry 4693 (class 1259 OID 131969)
-- Name: IX_Applications_ApplicationDate; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_Applications_ApplicationDate" ON public."Applications" USING btree ("ApplicationDate");


--
-- TOC entry 4694 (class 1259 OID 131968)
-- Name: IX_Applications_Number; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_Applications_Number" ON public."Applications" USING btree ("Number");


--
-- TOC entry 4695 (class 1259 OID 131971)
-- Name: IX_Applications_OrganizationUnit; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_Applications_OrganizationUnit" ON public."Applications" USING btree ("OrganizationUnit");


--
-- TOC entry 4696 (class 1259 OID 131970)
-- Name: IX_Applications_Status; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_Applications_Status" ON public."Applications" USING btree ("Status");


--
-- TOC entry 4710 (class 1259 OID 115549)
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- TOC entry 4713 (class 1259 OID 115551)
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- TOC entry 4716 (class 1259 OID 115552)
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- TOC entry 4719 (class 1259 OID 115553)
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- TOC entry 4706 (class 1259 OID 115556)
-- Name: IX_StatusHistory_ApplicationId; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_StatusHistory_ApplicationId" ON public."StatusHistory" USING btree ("ApplicationId");


--
-- TOC entry 4707 (class 1259 OID 131972)
-- Name: IX_StatusHistory_ChangedDate; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE INDEX "IX_StatusHistory_ChangedDate" ON public."StatusHistory" USING btree ("ChangedDate");


--
-- TOC entry 4701 (class 1259 OID 115550)
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- TOC entry 4705 (class 1259 OID 115555)
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: transport_user
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- TOC entry 4726 (class 2606 OID 115490)
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- TOC entry 4727 (class 2606 OID 115503)
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 4728 (class 2606 OID 115515)
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 4729 (class 2606 OID 115527)
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- TOC entry 4730 (class 2606 OID 115532)
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 4731 (class 2606 OID 115544)
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- TOC entry 4724 (class 2606 OID 131963)
-- Name: StatusHistory FK_StatusHistory_Applications; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."StatusHistory"
    ADD CONSTRAINT "FK_StatusHistory_Applications" FOREIGN KEY ("ApplicationId") REFERENCES public."Applications"("Id") ON DELETE CASCADE;


--
-- TOC entry 4725 (class 2606 OID 115477)
-- Name: StatusHistory FK_StatusHistory_Applications_ApplicationId; Type: FK CONSTRAINT; Schema: public; Owner: transport_user
--

ALTER TABLE ONLY public."StatusHistory"
    ADD CONSTRAINT "FK_StatusHistory_Applications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES public."Applications"("Id") ON DELETE CASCADE;


--
-- TOC entry 4898 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: pg_database_owner
--

GRANT ALL ON SCHEMA public TO transport_user;


-- Completed on 2026-03-20 10:55:07

--
-- PostgreSQL database dump complete
--

