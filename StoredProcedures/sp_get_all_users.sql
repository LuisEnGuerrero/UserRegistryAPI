CREATE OR REPLACE PROCEDURE sp_get_all_users()
LANGUAGE plpgsql AS $$
BEGIN
    PERFORM * FROM usuario;
END;
$$;
