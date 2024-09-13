CREATE OR REPLACE FUNCTION sp_create_auth_user(
    p_username VARCHAR,
    p_email VARCHAR,
    p_password_hash VARCHAR,
    p_role VARCHAR,
    p_is_google_auth BOOLEAN,
    p_google_id VARCHAR DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    INSERT INTO auth_users (username, email, password_hash, role, is_google_auth, google_id)
    VALUES (p_username, p_email, p_password_hash, p_role, p_is_google_auth, p_google_id);
END;
$$ LANGUAGE plpgsql;
