--SET IDENTITY_INSERT [Positions] ON;
--GO

--SET IDENTITY_INSERT [Players] ON;
--GO

-- [AAV]
--ALTER TABLE [dbo].[AAV] DROP CONSTRAINT [FK_AAV_Players_PlayerID]

ALTER TABLE [dbo].[AAV]  WITH CHECK ADD  CONSTRAINT [FK_AAV_Players_PlayerID] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

-- [ADP]
--ALTER TABLE [dbo].[ADP] DROP CONSTRAINT [FK_ADP_Players_PlayerID]

ALTER TABLE [dbo].[ADP]  WITH CHECK ADD  CONSTRAINT [FK_ADP_Players_PlayerID] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

-- [DVDB]
--ALTER TABLE [dbo].[DVDB] DROP CONSTRAINT [FK_DVDB_Players_PlayerID]

ALTER TABLE [dbo].[DVDB]  WITH CHECK ADD  CONSTRAINT [FK_DVDB_Players_PlayerID] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

-- [DepthChart]
--ALTER TABLE [dbo].[DepthChart] DROP CONSTRAINT [FK_DepthChart_Players_PlayerID]

ALTER TABLE [dbo].[DepthChart]  WITH CHECK ADD  CONSTRAINT [FK_DepthChart_Players_PlayerID] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

-- [Injuries]
--ALTER TABLE [dbo].[Injuries] DROP CONSTRAINT [FK_Injuries_Players_PlayerId]

ALTER TABLE [dbo].[Injuries]  WITH CHECK ADD  CONSTRAINT [FK_Injuries_Players_PlayerId] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

-- [PlayerNews]
--ALTER TABLE [dbo].[PlayerNews] DROP CONSTRAINT [FK_PlayerNews_Players_PlayerID]

ALTER TABLE [dbo].[PlayerNews]  WITH CHECK ADD  CONSTRAINT [FK_PlayerNews_Players_PlayerID] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

-- [Points]
--ALTER TABLE [dbo].[Points] DROP CONSTRAINT [FK_Points_Players_PlayerID]

ALTER TABLE [dbo].[Points]  WITH CHECK ADD  CONSTRAINT [FK_Points_Players_PlayerID] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO

--SET IDENTITY_INSERT [Positions] OFF;
--GO

SET IDENTITY_INSERT [Players] OFF;
GO