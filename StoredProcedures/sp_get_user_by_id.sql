-- Función para obtener un usuario por su ID
CREATE OR REPLACE FUNCTION sp_get_user_by_id(
    p_id INT
)
RETURNS SETOF usuario AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        id, 
        nombre AS name,          -- Usamos alias aquí también
        telefono AS phone, 
        direccion AS address, 
        pais_id AS countryId, 
        departamento_id AS departmentId, 
        municipio_id AS municipalityId
    FROM usuario
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
