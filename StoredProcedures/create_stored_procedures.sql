CREATE OR REPLACE PROCEDURE sp_create_user(
    _nombre VARCHAR(100),
    _telefono VARCHAR(15),
    _direccion VARCHAR(200),
    _pais_id INT,
    _departamento_id INT,
    _municipio_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO usuario (nombre, telefono, direccion, pais_id, departamento_id, municipio_id)
    VALUES (_nombre, _telefono, _direccion, _pais_id, _departamento_id, _municipio_id);
END;
$$;
