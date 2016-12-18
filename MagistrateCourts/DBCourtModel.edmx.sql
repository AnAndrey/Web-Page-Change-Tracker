
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/18/2016 12:57:47
-- Generated from EDMX file: C:\dotNet\projects\Practice\Genesis\GenesisTrialTest-Copy(2)\MagistrateCourts\DBCourtModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [NewBD];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CourtDistrcits_CourtRegion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CourtDistrcits] DROP CONSTRAINT [FK_CourtDistrcits_CourtRegion];
GO
IF OBJECT_ID(N'[dbo].[FK_CourtLocation_CourtDistricts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CourtLocation] DROP CONSTRAINT [FK_CourtLocation_CourtDistricts];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CourtDistrcits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CourtDistrcits];
GO
IF OBJECT_ID(N'[dbo].[CourtLocation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CourtLocation];
GO
IF OBJECT_ID(N'[dbo].[CourtRegion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CourtRegion];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CourtDistricts'
CREATE TABLE [dbo].[CourtDistricts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [WebSite] nvarchar(100)  NOT NULL,
    [RegionId] int  NOT NULL
);
GO

-- Creating table 'CourtLocations'
CREATE TABLE [dbo].[CourtLocations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NULL,
    [DistrictId] int  NOT NULL
);
GO

-- Creating table 'CourtRegions'
CREATE TABLE [dbo].[CourtRegions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Number] nvarchar(10)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'CourtDistricts'
ALTER TABLE [dbo].[CourtDistricts]
ADD CONSTRAINT [PK_CourtDistricts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CourtLocations'
ALTER TABLE [dbo].[CourtLocations]
ADD CONSTRAINT [PK_CourtLocations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CourtRegions'
ALTER TABLE [dbo].[CourtRegions]
ADD CONSTRAINT [PK_CourtRegions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [RegionId] in table 'CourtDistricts'
ALTER TABLE [dbo].[CourtDistricts]
ADD CONSTRAINT [FK_CourtDistrcits_CourtRegion]
    FOREIGN KEY ([RegionId])
    REFERENCES [dbo].[CourtRegions]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CourtDistrcits_CourtRegion'
CREATE INDEX [IX_FK_CourtDistrcits_CourtRegion]
ON [dbo].[CourtDistricts]
    ([RegionId]);
GO

-- Creating foreign key on [DistrictId] in table 'CourtLocations'
ALTER TABLE [dbo].[CourtLocations]
ADD CONSTRAINT [FK_CourtLocation_CourtDistricts]
    FOREIGN KEY ([DistrictId])
    REFERENCES [dbo].[CourtDistricts]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CourtLocation_CourtDistricts'
CREATE INDEX [IX_FK_CourtLocation_CourtDistricts]
ON [dbo].[CourtLocations]
    ([DistrictId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------