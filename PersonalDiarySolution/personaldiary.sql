USE [PersonalDiaryDB]
GO
/****** Object:  Table [dbo].[Comments]    Script Date: 3/29/2025 7:58:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[CommentId] [int] IDENTITY(1,1) NOT NULL,
	[EntryId] [int] NOT NULL,
	[UserId] [int] NULL,
	[GuestName] [nvarchar](50) NULL,
	[Content] [ntext] NOT NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiaryEntries]    Script Date: 3/29/2025 7:58:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiaryEntries](
	[EntryId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Content] [ntext] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[Mood] [nvarchar](50) NULL,
	[Weather] [nvarchar](50) NULL,
	[IsPublic] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[EntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiaryEntryTags]    Script Date: 3/29/2025 7:58:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiaryEntryTags](
	[EntryId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EntryId] ASC,
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 3/29/2025 7:58:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tags](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[TagName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/29/2025 7:58:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Comments] ON 

INSERT [dbo].[Comments] ([CommentId], [EntryId], [UserId], [GuestName], [Content], [CreatedDate]) VALUES (1, 1, 4, NULL, N'Chúc bạn có một khóa học tốt', CAST(N'2025-03-26T21:47:04.973' AS DateTime))
INSERT [dbo].[Comments] ([CommentId], [EntryId], [UserId], [GuestName], [Content], [CreatedDate]) VALUES (2, 8, 4, NULL, N'Test comment', CAST(N'2025-03-28T04:34:08.657' AS DateTime))
INSERT [dbo].[Comments] ([CommentId], [EntryId], [UserId], [GuestName], [Content], [CreatedDate]) VALUES (4, 8, NULL, N'Anonimos1', N'test comment guest', CAST(N'2025-03-28T05:15:41.403' AS DateTime))
INSERT [dbo].[Comments] ([CommentId], [EntryId], [UserId], [GuestName], [Content], [CreatedDate]) VALUES (5, 6, 4, NULL, N'tester2 cmt to admin entry diary', CAST(N'2025-03-28T11:24:53.960' AS DateTime))
SET IDENTITY_INSERT [dbo].[Comments] OFF
GO
SET IDENTITY_INSERT [dbo].[DiaryEntries] ON 

INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (1, 1, N'Ngày đầu tiên với ASP.NET', N'Hôm nay tôi bắtt đầu học ASP.NET. Thật thú vị!', CAST(N'2025-03-11T08:22:03.003' AS DateTime), NULL, N'Hào hứng', N'Nắng', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (2, 1, N'Suy nghi cá nhân', N'Ðây là bài viết riêng tư...', CAST(N'2025-03-11T08:22:03.003' AS DateTime), NULL, N'Trầm lặng', N'Mưa', 0)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (3, 1, N'hello today', N'hello test diary', CAST(N'2025-03-25T10:39:46.140' AS DateTime), NULL, N'happy', N'sunny', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (4, 1, N'new diary 1', N'hello diary', CAST(N'2025-03-25T10:44:16.313' AS DateTime), NULL, N'sad', N'rainy', 0)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (5, 1, N'new diary 2', N'hello diary', CAST(N'2025-03-25T10:44:52.833' AS DateTime), NULL, N'sad', N'rainy', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (6, 1, N'new diary 2', N'ssd', CAST(N'2025-03-25T10:52:13.210' AS DateTime), NULL, N'sad', N'rainy', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (7, 4, N'create diary teaster2', N'gg', CAST(N'2025-03-26T15:14:58.957' AS DateTime), CAST(N'2025-03-26T21:12:59.143' AS DateTime), N'happy', N'rainy', 0)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (8, 4, N'create diary tester2', N'today my fam visit me', CAST(N'2025-03-26T15:15:58.640' AS DateTime), NULL, N'angry', N'sunny', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (9, 4, N'public diary 2 by tt2', N'hello tomorrow i go to school', CAST(N'2025-03-28T06:00:03.633' AS DateTime), NULL, N'happy', N'rainny', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (10, 4, N'diary with auto fill tag', N'tester 2 test create tag auto fill', CAST(N'2025-03-28T06:22:48.677' AS DateTime), CAST(N'2025-03-28T06:47:08.813' AS DateTime), N'happy', N'rainy', 1)
INSERT [dbo].[DiaryEntries] ([EntryId], [UserId], [Title], [Content], [CreatedDate], [ModifiedDate], [Mood], [Weather], [IsPublic]) VALUES (11, 5, N'diary entry tester3 ', N'this created from tester 3 ', CAST(N'2025-03-28T11:33:27.157' AS DateTime), NULL, N'exited', N'snowy', 1)
SET IDENTITY_INSERT [dbo].[DiaryEntries] OFF
GO
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (1, 1)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (1, 2)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (2, 3)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (3, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (4, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (5, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (6, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (7, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (8, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (9, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (10, 1)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (10, 4)
INSERT [dbo].[DiaryEntryTags] ([EntryId], [TagId]) VALUES (11, 5)
GO
SET IDENTITY_INSERT [dbo].[Tags] ON 

INSERT [dbo].[Tags] ([TagId], [TagName]) VALUES (1, N'ASP.NET')
INSERT [dbo].[Tags] ([TagId], [TagName]) VALUES (3, N'Cá nhân')
INSERT [dbo].[Tags] ([TagId], [TagName]) VALUES (5, N'cinema')
INSERT [dbo].[Tags] ([TagId], [TagName]) VALUES (4, N'daily')
INSERT [dbo].[Tags] ([TagId], [TagName]) VALUES (2, N'Học tập')
SET IDENTITY_INSERT [dbo].[Tags] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [CreatedDate], [LastLoginDate], [IsActive]) VALUES (1, N'admin', N'admin@example.com', N'AQAAAAEAACcQAAAAEGScL6e+jQZazU9TlL5L28aWL/zLlBBHRXN5RMVv7ZbESox72hebQOb8Wq7+53jDnA==', N'Admin User', CAST(N'2025-03-11T08:22:03.000' AS DateTime), NULL, 1)
INSERT [dbo].[Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [CreatedDate], [LastLoginDate], [IsActive]) VALUES (2, N'K9Gnoah', N'maihuyhoang0703@gmail.com', N'iqPOUGc7IFr/gBl6Nc/GTx2ncjr3ri+juNnhe4P2oYo=', N'Mai Huy Hoang', CAST(N'2025-03-21T07:31:03.563' AS DateTime), CAST(N'2025-03-25T11:55:07.030' AS DateTime), 1)
INSERT [dbo].[Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [CreatedDate], [LastLoginDate], [IsActive]) VALUES (3, N'tester', N'abc@gmail.com', N'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', N'Tester', CAST(N'2025-03-25T11:26:43.427' AS DateTime), CAST(N'2025-03-25T11:55:19.133' AS DateTime), 1)
INSERT [dbo].[Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [CreatedDate], [LastLoginDate], [IsActive]) VALUES (4, N'tester2', N'abcd@gmail.com', N'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', N'MyTester2', CAST(N'2025-03-25T12:02:11.150' AS DateTime), CAST(N'2025-03-28T11:23:43.097' AS DateTime), 1)
INSERT [dbo].[Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [CreatedDate], [LastLoginDate], [IsActive]) VALUES (5, N'tester3', N'xyz@gmail.com', N'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', N'TESTER3', CAST(N'2025-03-28T08:18:08.940' AS DateTime), CAST(N'2025-03-28T11:31:22.503' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Tags__BDE0FD1D8926F347]    Script Date: 3/29/2025 7:58:24 AM ******/
ALTER TABLE [dbo].[Tags] ADD UNIQUE NONCLUSTERED 
(
	[TagName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E48D2DE83E]    Script Date: 3/29/2025 7:58:24 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D10534153000F9]    Script Date: 3/29/2025 7:58:24 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Comments] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[DiaryEntries] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[DiaryEntries] ADD  DEFAULT ((0)) FOR [IsPublic]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD FOREIGN KEY([EntryId])
REFERENCES [dbo].[DiaryEntries] ([EntryId])
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DiaryEntries]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DiaryEntryTags]  WITH CHECK ADD FOREIGN KEY([EntryId])
REFERENCES [dbo].[DiaryEntries] ([EntryId])
GO
ALTER TABLE [dbo].[DiaryEntryTags]  WITH CHECK ADD FOREIGN KEY([TagId])
REFERENCES [dbo].[Tags] ([TagId])
GO
