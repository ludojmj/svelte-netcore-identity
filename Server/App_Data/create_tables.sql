DROP TABLE IF EXISTS t_stuff;
DROP TABLE IF EXISTS t_user;
CREATE TABLE t_user
(
    usr_id            TEXT NOT NULL PRIMARY KEY UNIQUE,
    usr_name          TEXT,
    usr_given_name    TEXT,
    usr_family_name   TEXT,
    usr_email         TEXT,
    usr_created_at    TEXT,
    usr_updated_at    TEXT
);
CREATE TABLE t_stuff
(
    stf_id            TEXT NOT NULL PRIMARY KEY UNIQUE,
    stf_user_id       TEXT NOT NULL,
    stf_label         TEXT NOT NULL,
    stf_description   TEXT,
    stf_other_info    TEXT,
    stf_created_at    TEXT,
    stf_updated_at    TEXT,
    FOREIGN KEY (stf_user_id)
      REFERENCES t_user (usr_id)
         ON DELETE CASCADE
         ON UPDATE NO ACTION
);
