CREATE TABLE dbo.Profile
(
	[Id] TINYINT NOT NULL DEFAULT 1, 
    [EmailFirst] VARCHAR(MAX) NOT NULL, 
    [EmailSecond] VARCHAR(MAX) NOT NULL, 
    [Password] VARCHAR(MAX) NOT NULL,
	CONSTRAINT Id_PK 
    PRIMARY KEY ([Id]),
    CONSTRAINT Id_OnlyOneRow 
    CHECK ([Id] = 1)
)
