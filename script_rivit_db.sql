create table if not exists authorities
(
    username  varchar(50) not null,
    authority varchar(50) not null
);

alter table authorities
    owner to postgres;

create unique index if not exists ix_auth_username
    on authorities (username, authority);

create table if not exists demographic
(
    id_demographic integer default nextval('"demographic _id_demographic_seq"'::regclass) not null
        constraint "demographic _pk"
            primary key,
    edad           integer,
    sexo           varchar,
    escolaridad    varchar,
    origen         varchar,
    procedencia    varchar,
    residencia     varchar,
    religion       varchar,
    lat_manual     varchar,
    fk_id_user     integer
        constraint "demographic _users_id_fk"
            references users
);

alter table demographic
    owner to postgres;

create unique index if not exists "demographic _id_demographic_uindex"
    on demographic (id_demographic);

create unique index if not exists "demographic _fk_id_user_uindex"
    on demographic (fk_id_user);

create table if not exists game
(
    id_juego  serial
        constraint game_pk
            primary key,
    nombre    varchar,
    max_nivel integer default 0
);

alter table game
    owner to postgres;

create unique index if not exists game_id_juego_uindex
    on game (id_juego);

create unique index if not exists game_nombre_uindex
    on game (nombre);

create table if not exists metric
(
    id_metric     serial
        constraint metric_pk
            primary key,
    fk_id_juego   integer
        constraint id_juego___fk
            references game,
    data_json     varchar,
    fk_id_session integer
);

alter table metric
    owner to postgres;

create unique index if not exists metric_id_metric_uindex
    on metric (id_metric);

create table if not exists metric_game
(
    id_metric_game serial
        constraint metric_game_pk
            primary key,
    fk_id_metric   integer
        constraint metric_game_metric_id_metric_fk
            references metric,
    fk_id_game     integer
        constraint metric_game_game_id_juego_fk
            references game
);

alter table metric_game
    owner to postgres;

create unique index if not exists metric_game_id_metric_game_uindex
    on metric_game (id_metric_game);

create table if not exists role
(
    id         integer default nextval('roles_id_seq'::regclass) not null
        constraint roles_pkey
            primary key,
    fk_user_id integer                                           not null
        constraint fk_user_id
            references users,
    rol        varchar(45)                                       not null
);

alter table role
    owner to postgres;

create index if not exists fki_user_id
    on role (fk_user_id);

create table if not exists roles
(
    role_id serial
        constraint roles_pk
            primary key,
    name    varchar not null
);

alter table roles
    owner to postgres;

create unique index if not exists roles_name_uindex
    on roles (name);

create table if not exists session
(
    id_session    serial
        constraint session_pk
            primary key,
    start_session timestamp,
    end_session   timestamp,
    fk_id_user    integer
        constraint session_users_id_fk
            references users
);

alter table session
    owner to postgres;

create unique index if not exists session_id_session_uindex
    on session (id_session);

create table if not exists user_session_metric
(
    id            serial
        constraint user_session_metric_pk
            primary key,
    fk_id_user    integer,
    fk_id_session integer,
    fk_id_metric  integer,
    value         json
);

alter table user_session_metric
    owner to postgres;

create unique index if not exists user_session_metric_id_uindex
    on user_session_metric (id);


create table if not exists users
(
    id            serial
        primary key,
    username      varchar(45)        not null,
    password      varchar(60)        not null,
    email         varchar(100)       not null,
    register      date               not null,
    lastconection timestamp,
    enabled       smallint default 1 not null
);

alter table users
    owner to postgres;

create unique index if not exists users_username_uindex
    on users (username);

create table if not exists users_roles
(
    user_id integer
        constraint users___fk
            references users
            on update cascade on delete cascade,
    role_id integer
        constraint roles___fk
            references roles
            on update cascade on delete cascade
);

alter table users_roles
    owner to postgres;