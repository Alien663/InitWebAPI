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

create procedure [dbo].[xp_TTupdate]
	@TID int,
	@TName nvarchar(4),
	@TDes varchar(16)
as begin
	update TT
		set TName = @TName, TDes = @TDes
		where TID = @TID
end
go

create procedure [dbo].[xp_TTinsert]
	@TID int output,
	@TName nvarchar(4),
	@TDes varchar(16)
as begin
	insert into TT(TName, TDes)
		values(@TName, @TDes)
	set @TID = SCOPE_IDENTITY()
end
go

create procedure xp_TTdelete
	@TID int output
as begin
	delete from TT where TID = @TID
end
go