-- Procedimiento para eliminar un usuario por su ID
CREATE OR REPLACE FUNCTION sp_delete_user(
    p_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM usuario WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
