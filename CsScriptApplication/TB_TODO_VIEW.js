var mainSql = `
    SELECT *
    FROM TB_TODO WITH(NOLOCK)
    WHERE USERID = @USERID
`;

if(v_date != null) {
    mainSql = mainSql + ` AND WRITE_DT = @V_DATE `;
}

if(v_id > 0) {
    mainSql += ` AND ID = @V_ID `;
}