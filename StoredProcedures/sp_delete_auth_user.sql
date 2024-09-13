CREATE OR REPLACE FUNCTION sp_delete_auth_user(p_id INT)
RETURNS VOID AS $$
BEGIN
    DELETE FROM auth_users WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
