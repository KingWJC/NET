CREATE TABLE JHICUV3.BASE_USER(
	Id NUMBER(10)  NOT NULL,
	Name VARCHAR2(50) NULL,
	LoginName VARCHAR2(100) NOT NULL,
	Password VARCHAR2(100) NOT NULL,
	Email VARCHAR2(200) NULL,
	Mobile VARCHAR2(30) NULL,
	State NUMBER(2) NOT NULL,
	UserType NUMBER(2) NOT NULL,
	LastLoginTime DATE NULL,
	CreateTime DATE NOT NULL,
	CreatorId NUMBER(2) NOT NULL,
	LastModifierId NUMBER(2) NULL,
	LastModifyTime DATE NULL
)

comment on column JHICUV3.BASE_USER.Id
  is '�û����';
comment on column JHICUV3.BASE_USER.Name
  is '����';
comment on column JHICUV3.BASE_USER.LoginName
  is '��¼��';
comment on column JHICUV3.BASE_USER.Password
  is '����';
comment on column JHICUV3.BASE_USER.Email
  is '����';
comment on column JHICUV3.BASE_USER.Mobile
  is '�ֻ���';
comment on column JHICUV3.BASE_USER.State
  is '�û�״̬  0���� 1���� 2ɾ��';
comment on column JHICUV3.BASE_USER.UserType
  is '�û�����  1 ��ͨ�û� 2����Ա 4��������Ա';
comment on column JHICUV3.BASE_USER.LastLoginTime
  is '����¼ʱ��';
comment on column JHICUV3.BASE_USER.CreateTime
  is '����ʱ��';
comment on column JHICUV3.BASE_USER.CreatorId
  is '������';
 comment on column JHICUV3.BASE_USER.LastModifierId
  is '�޸���';
 comment on column JHICUV3.BASE_USER.LastModifyTime
  is '�޸�ʱ��';
																																						
create sequence JHICUV3.BASE_USER_ID
minvalue 1
maxvalue 9999999999999999999999999999
start with 1
increment by 1
cache 10
order;
 
 INSERT INTO BASE_User
           (ID
           ,Name
           ,LoginName
           ,Password
           ,Email
           ,Mobile
           ,State
           ,UserType
           ,LastLoginTime
           ,CreateTime
           ,CreatorId
           ,LastModifierId
           ,LastModifyTime)
     VALUES
           (BASE_USER_ID.NEXTVAL
           ,'С��'
           ,'admin'
           ,'e10adc3949ba59abbe56e057f20f883e'
           ,'12'
           ,'133'	
           ,0
           ,2
           ,TO_DATE('2022-04-10 08:00:00','yyyy-mm-dd HH24:mi:ss')
           ,to_date('2022-04-22 12:12:12','yyyy-mm-dd HH24:mi:ss')
           ,1
           ,1
           ,to_date('2022-12-12','yyyy-mm-dd HH24:mi:ss'))

SELECT * FROM base_user
