-- Función para obtener todos los usuarios
CREATE OR REPLACE FUNCTION sp_get_all_users()
RETURNS SETOF usuario AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        id, 
        nombre AS name,         -- Usamos alias para coincidir con las propiedades del modelo
        telefono AS phone, 
        direccion AS address, 
        pais_id AS countryId, 
        departamento_id AS departmentId, 
        municipio_id AS municipalityId
    FROM usuario;
END;
$$ LANGUAGE plpgsql;
