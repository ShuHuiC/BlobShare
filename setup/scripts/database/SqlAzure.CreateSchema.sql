/****** Object:  Table [dbo].[Roles]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](256) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[BlobSets]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlobSets](
	[BlobSetId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ParentBlobSetId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_BlobSets] PRIMARY KEY CLUSTERED 
(
	[BlobSetId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Users]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[IdentityProvider] [nvarchar](128) NULL,
	[NameIdentifier] [nvarchar](256) NULL,
	[Inactive] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Blobs]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Blobs](
	[BlobId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[OriginalFileName] [nvarchar](200) NOT NULL,
	[UploadDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Contents] PRIMARY KEY CLUSTERED 
(
	[BlobId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[UsersRoles]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserInRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[BlobEvents]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlobEvents](
	[BlobEventId] [uniqueidentifier] NOT NULL,
	[BlobId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[EventType] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[Url] [nvarchar](200) NULL,
	[UserAgent] [nvarchar](max) NULL,
	[RemoteMachine] [nvarchar](100) NULL,
	[SessionId] [nvarchar](100) NULL,
 CONSTRAINT [PK_BlobEvents] PRIMARY KEY CLUSTERED 
(
	[BlobEventId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[UserEvents]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserEvents](
	[UserEventId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[EventType] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[Url] [nvarchar](200) NULL,
	[UserAgent] [nvarchar](MAX) NULL,
	[RemoteMachine] [nvarchar](100) NULL,
	[SessionId] [nvarchar](100) NULL,
 CONSTRAINT [PK_UserEvents] PRIMARY KEY CLUSTERED 
(
	[UserEventId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[BlobSetEvents]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlobSetEvents](
	[BlobSetEventId] [uniqueidentifier] NOT NULL,
	[BlobSetId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[EventType] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[Url] [nvarchar](200) NULL,
	[UserAgent] [nvarchar](max) NULL,
	[RemoteMachine] [nvarchar](100) NULL,
	[SessionId] [nvarchar](100) NULL,
 CONSTRAINT [PK_BlobSetEvents] PRIMARY KEY CLUSTERED 
(
	[BlobSetEventId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[PermissionId] [uniqueidentifier] NOT NULL,
	[BlobId] [uniqueidentifier] NULL,
	[BlobSetId] [uniqueidentifier] NULL,
	[CreationDateTime] [datetime] NOT NULL,
	[ExpirationDateTime] [datetime] NOT NULL,
	[Privilege] [int] NOT NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[Invitations]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invitations](
	[InvitationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Privilege] [int] NOT NULL,
	[CreationDateTime] [datetime] NOT NULL,
	[ExpirationDateTime] [datetime] NOT NULL,
	[SentDateTime] [datetime] NULL,
	[ActivationDateTime] [datetime] NULL,
 CONSTRAINT [PK_Invitations] PRIMARY KEY CLUSTERED 
(
	[InvitationId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[BlobSetBlobs]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlobSetBlobs](
	[BlobSetId] [uniqueidentifier] NOT NULL,
	[BlobId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_BlobList] PRIMARY KEY CLUSTERED 
(
	[BlobSetId] ASC,
	[BlobId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[UserPermissions]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPermissions](
	[UserId] [uniqueidentifier] NOT NULL,
	[PermissionId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[PermissionId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 06/08/2011 18:38:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[RoleId] [uniqueidentifier] NOT NULL,
	[PermissionId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[PermissionId] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
/****** Object:  Default [DF_Blobs_BlobId]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Blobs] ADD  CONSTRAINT [DF_Blobs_BlobId]  DEFAULT (newid()) FOR [BlobId]
GO
/****** Object:  Default [DF_Invitations_InvitationId]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Invitations] ADD  CONSTRAINT [DF_Invitations_InvitationId]  DEFAULT (newid()) FOR [InvitationId]
GO
/****** Object:  Default [DF_Permissions_PermissionId]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Permissions] ADD  CONSTRAINT [DF_Permissions_PermissionId]  DEFAULT (newid()) FOR [PermissionId]
GO
/****** Object:  Default [DF_Permissions_Priviliges]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Permissions] ADD  CONSTRAINT [DF_Permissions_Priviliges]  DEFAULT ((1)) FOR [Privilege]
GO
/****** Object:  Default [DF_BlobSets_BlobSetId]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobSets] ADD  CONSTRAINT [DF_BlobSets_BlobSetId]  DEFAULT (newid()) FOR [BlobSetId]
GO
/****** Object:  Default [DF_Roles_RoleId]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_RoleId]  DEFAULT (newid()) FOR [RoleId]
GO
/****** Object:  Default [DF_Users_UserId]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_UserId]  DEFAULT (newid()) FOR [UserId]
GO
/****** Object:  Default [DF_Users_Name]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Name]  DEFAULT (NULL) FOR [Name]
GO
/****** Object:  Default [DF_Users_Inactive]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Inactive]  DEFAULT ((0)) FOR [Inactive]
GO
/****** Object:  ForeignKey [FK_BlobEvents_Blobs]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobEvents]  WITH CHECK ADD  CONSTRAINT [FK_BlobEvents_Blobs] FOREIGN KEY([BlobId])
REFERENCES [dbo].[Blobs] ([BlobId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlobEvents] CHECK CONSTRAINT [FK_BlobEvents_Blobs]
GO
/****** Object:  ForeignKey [FK_BlobEvents_Users]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobEvents]  WITH CHECK ADD  CONSTRAINT [FK_BlobEvents_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlobEvents] CHECK CONSTRAINT [FK_BlobEvents_Users]
GO
/****** Object:  ForeignKey [FK_BlobSetBlobs_Blobs]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobSetBlobs]  WITH CHECK ADD  CONSTRAINT [FK_BlobSetBlobs_Blobs] FOREIGN KEY([BlobId])
REFERENCES [dbo].[Blobs] ([BlobId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlobSetBlobs] CHECK CONSTRAINT [FK_BlobSetBlobs_Blobs]
GO
/****** Object:  ForeignKey [FK_BlobSetBlobs_BlobSets]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobSetBlobs]  WITH CHECK ADD  CONSTRAINT [FK_BlobSetBlobs_BlobSets] FOREIGN KEY([BlobSetId])
REFERENCES [dbo].[BlobSets] ([BlobSetId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlobSetBlobs] CHECK CONSTRAINT [FK_BlobSetBlobs_BlobSets]
GO
/****** Object:  ForeignKey [FK_Invitations_Users]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Invitations]  WITH CHECK ADD  CONSTRAINT [FK_Invitations_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Invitations] CHECK CONSTRAINT [FK_Invitations_Users]
GO
/****** Object:  ForeignKey [FK_Permissions_Blobs]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_Permissions_Blobs] FOREIGN KEY([BlobId])
REFERENCES [dbo].[Blobs] ([BlobId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Permissions] CHECK CONSTRAINT [FK_Permissions_Blobs]
GO
/****** Object:  ForeignKey [FK_Permissions_BlobSets]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_Permissions_BlobSets] FOREIGN KEY([BlobSetId])
REFERENCES [dbo].[BlobSets] ([BlobSetId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Permissions] CHECK CONSTRAINT [FK_Permissions_BlobSets]
GO
/****** Object:  ForeignKey [FK_BlobSetEvents_BlobSets]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobSetEvents]  WITH CHECK ADD  CONSTRAINT [FK_BlobSetEvents_BlobSets] FOREIGN KEY([BlobSetId])
REFERENCES [dbo].[BlobSets] ([BlobSetId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlobSetEvents] CHECK CONSTRAINT [FK_BlobSetEvents_BlobSets]
GO
/****** Object:  ForeignKey [FK_BlobSetEvents_Users]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[BlobSetEvents]  WITH CHECK ADD  CONSTRAINT [FK_BlobSetEvents_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlobSetEvents] CHECK CONSTRAINT [FK_BlobSetEvents_Users]
GO
/****** Object:  ForeignKey [FK_RolePermissions_Permissions]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permissions] ([PermissionId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Permissions]
GO
/****** Object:  ForeignKey [FK_RolePermissions_Roles]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Roles]
GO
/****** Object:  ForeignKey [FK_UserEvents_Users]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[UserEvents]  WITH CHECK ADD  CONSTRAINT [FK_UserEvents_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserEvents] CHECK CONSTRAINT [FK_UserEvents_Users]
GO
/****** Object:  ForeignKey [FK_UserPermissions_Permissions]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[UserPermissions]  WITH CHECK ADD  CONSTRAINT [FK_UserPermissions_Permissions] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permissions] ([PermissionId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserPermissions] CHECK CONSTRAINT [FK_UserPermissions_Permissions]
GO
/****** Object:  ForeignKey [FK_UserPermissions_Users]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[UserPermissions]  WITH CHECK ADD  CONSTRAINT [FK_UserPermissions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserPermissions] CHECK CONSTRAINT [FK_UserPermissions_Users]
GO
/****** Object:  ForeignKey [FK_UsersRoles_Roles]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[UsersRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersRoles] CHECK CONSTRAINT [FK_UsersRoles_Roles]
GO
/****** Object:  ForeignKey [FK_UsersRoles_Users]    Script Date: 06/08/2011 18:38:54 ******/
ALTER TABLE [dbo].[UsersRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersRoles] CHECK CONSTRAINT [FK_UsersRoles_Users]
GO