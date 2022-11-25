create table TT
(
	TID int identity(1, 1),
	TName nvarchar(4),
	TDes varchar(16),
	constraint PK_TT primary key(TID)
)
go

insert into TT(TName, TDes)
	values
		('我不會飛', 'abcd'),
		('abcd', 'abcd'),
		('A1', 'B1'),
		('A2', 'B2'),
		('A3', 'B3'),
		('A4', 'B4')
go