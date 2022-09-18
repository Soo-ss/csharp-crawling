-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

drop table if exists [dbo].[tbmskim];
drop index if exists idx_t1 on dbo.tbmskim;

create table dbo.tbmskim
(
	id bigint identity(1,1) not null primary key,
	channelType varchar(9) not null check (channelType in('clien', 'instagram')),
	contentsType varchar(7) not null check (contentsType in('main', 'comment')),
	checkCommentId varchar(255),
	writer varchar(255),
	contents varchar(max),
	created datetime,
	tag varchar(max),
	commentCount int,
	url varchar(255),
	profilePicture varchar(max),
	likes int,
	crawlingDate datetime,
	hits int,
	keyword varchar(255),
	pictureAndVideo varchar(max)
);

create unique index idx_t1 
on dbo.tbmskim (contentsType, url)
where contentsType = 'main';

-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
-- /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

select * from dbo.tbmskim;

select * from dbo.tbmskim 
where channelType = 'instagram'
and contentsType = 'main' 
--and keyword = ''
;

select * from dbo.tbmskim 
where channelType = 'instagram' 
and contentsType = 'comment' 
and keyword = '벨루가'
order by crawlingDate;

select * from dbo.tbmskim 
where channelType = 'instagram' and contentsType = 'comment' and keyword = '짬뽕';

insert into dbo.tbmskim (channelType, contentsType, checkCommentId, writer, contents, created, tag, commentCount, url, profilePicture, likes, crawlingDate, hits)
values ('instagram', 'comment', 'dsgwer', 'soaroom_', '선팔감사합니둥 맞팔했어용', '2022-08-22 16:07:55.000', '', 34, 'https://www.instagram.com/p/ChPRirJLeAS/', 'https://scontent-ssn1-1.cdninstagram.com/v/t51.2885-19/299418946_5367282736696589_7856827389271043400_n.jpg?stp=dst-jpg_s150x150&_nc_ht=scontent-ssn1-1.cdninstagram.com&_nc_cat=105&_nc_ohc=-n9078TTKksAX9vpaJh&edm=AFDWGO4BAAAA&ccb=7-5&oh=00_AT-UBtkkd5HgvpO1-ev53qQrmHzTS_lErmMZsqQoJKayuQ&oe=6309648B&_nc_sid=2ea7f4', 12, '2022-08-22 17:09:07.367', 0);

