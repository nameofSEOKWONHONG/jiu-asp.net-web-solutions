var sql = '';

// begin sql text
var select = 'SELECT * FROM TB_TODO WITH(NOLOCK)';
var where = 'WHERE USERID = @USERID ';

if(V_DATE != null) {
    where += 'AND WRITE_DT = @V_DATE ';
}

if(V_ID > 0) {
    where += 'AND ID = @V_ID ';
}
// end sql text

// sql result
sql = `${select} ${where}`;