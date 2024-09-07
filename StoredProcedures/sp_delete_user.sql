CREATE OR REPLACE PROCEDURE sp_delete_user(
    IN p_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    DELETE FROM usuario WHERE id = p_id;
END;
$$;
