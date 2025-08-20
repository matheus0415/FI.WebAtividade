CREATE PROC FI_SP_PesqBeneficiario
	@iniciarEm int,
	@quantidade int,
	@campoOrdenacao varchar(200),
	@crescente bit,
	@idCliente bigint = NULL
AS
BEGIN
	DECLARE @SCRIPT NVARCHAR(MAX)
	DECLARE @CAMPOS NVARCHAR(MAX)
	DECLARE @ORDER VARCHAR(50)
	DECLARE @WHERE VARCHAR(200)
	
	IF(@campoOrdenacao = 'CPF')
		SET @ORDER =  ' CPF '
	ELSE
		SET @ORDER = ' NOME '

	IF(@crescente = 0)
		SET @ORDER = @ORDER + ' DESC'
	ELSE
		SET @ORDER = @ORDER + ' ASC'

	SET @WHERE = ''
	IF(@idCliente IS NOT NULL)
		SET @WHERE = ' WHERE IDCLIENTE = ' + CAST(@idCliente AS VARCHAR(20))

	SET @CAMPOS = '@iniciarEm int,@quantidade int'
	SET @SCRIPT = 
	'SELECT ID, NOME, CPF, IDCLIENTE FROM
		(SELECT ROW_NUMBER() OVER (ORDER BY ' + @ORDER + ') AS Row, ID, NOME, CPF, IDCLIENTE FROM BENEFICIARIOS WITH(NOLOCK)' + @WHERE + ')
		AS BeneficiariosWithRowNumbers
	WHERE Row > @iniciarEm AND Row <= (@iniciarEm+@quantidade) ORDER BY'
    
	SET @SCRIPT = @SCRIPT + @ORDER
            
	EXECUTE SP_EXECUTESQL @SCRIPT, @CAMPOS, @iniciarEm, @quantidade

	IF(@idCliente IS NOT NULL)
		SELECT COUNT(1) FROM BENEFICIARIOS WITH(NOLOCK) WHERE IDCLIENTE = @idCliente
	ELSE
		SELECT COUNT(1) FROM BENEFICIARIOS WITH(NOLOCK)
END
