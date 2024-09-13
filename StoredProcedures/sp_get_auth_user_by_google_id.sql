CREATE OR REPLACE FUNCTION sp_get_auth_user_by_google_id(p_google_id VARCHAR)
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
    SELECT * FROM auth_users WHERE google_id = p_google_id;
END;
$$ LANGUAGE plpgsql;
