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
  is '用户编号';
comment on column JHICUV3.BASE_USER.Name
  is '姓名';
comment on column JHICUV3.BASE_USER.LoginName
  is '登录名';
comment on column JHICUV3.BASE_USER.Password
  is '密码';
comment on column JHICUV3.BASE_USER.Email
  is '邮箱';
comment on column JHICUV3.BASE_USER.Mobile
  is '手机号';
comment on column JHICUV3.BASE_USER.State
  is '用户状态  0正常 1冻结 2删除';
comment on column JHICUV3.BASE_USER.UserType
  is '用户类型  1 普通用户 2管理员 4超级管理员';
comment on column JHICUV3.BASE_USER.LastLoginTime
  is '最后登录时间';
comment on column JHICUV3.BASE_USER.CreateTime
  is '创建时间';
comment on column JHICUV3.BASE_USER.CreatorId
  is '创建人';
 comment on column JHICUV3.BASE_USER.LastModifierId
  is '修改人';
 comment on column JHICUV3.BASE_USER.LastModifyTime
  is '修改时间';
																																						
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
           ,'小新'
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
