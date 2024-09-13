CREATE OR REPLACE FUNCTION sp_get_all_auth_users()
RETURNS TABLE (
    id INT,
    username VARCHAR,
    email VARCHAR,
    password_hash VARCHAR,
    role VARCHAR,
    is_google_auth BOOLEAN,
    google_id VARCHAR,
    is_authorized BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM auth_users;
END;
$$ LANGUAGE plpgsql;
