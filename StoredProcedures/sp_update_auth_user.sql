CREATE OR REPLACE FUNCTION sp_update_auth_user(
    p_id INT,
    p_username VARCHAR,
    p_email VARCHAR,
    p_password_hash VARCHAR,
    p_role VARCHAR,
    p_is_google_auth BOOLEAN,
    p_is_authorized BOOLEAN,
    p_google_id VARCHAR DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    UPDATE auth_users
    SET 
        username = p_username,
        email = p_email,
        password_hash = p_password_hash,
        role = p_role,
        is_google_auth = p_is_google_auth,
        google_id = p_google_id,
        is_authorized = p_is_authorized
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
