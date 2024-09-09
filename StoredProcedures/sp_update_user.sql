-- Procedimiento para actualizar un usuario
CREATE OR REPLACE FUNCTION sp_update_user(
    p_id INT,
    p_nombre TEXT,
    p_telefono TEXT,
    p_direccion TEXT,
    p_pais_id INT,
    p_departamento_id INT,
    p_municipio_id INT
)
RETURNS VOID AS $$
BEGIN
    UPDATE usuario
    SET nombre = p_nombre,
        telefono = p_telefono,
        direccion = p_direccion,
        pais_id = p_pais_id,
        departamento_id = p_departamento_id,
        municipio_id = p_municipio_id
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
