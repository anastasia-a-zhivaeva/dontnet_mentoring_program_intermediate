/*
** Copyright Microsoft, Inc. 1994 - 2000
** All Rights Reserved.
*/

-- This script does not create a database.
-- Run this script in the database you want the objects to be created.
-- Default schema is dbo.

SET NOCOUNT ON
GO

set quoted_identifier on
GO

/* Set DATEFORMAT so that the date strings are interpreted correctly regardless of
   the default DATEFORMAT on the server.
*/
SET DATEFORMAT mdy
GO

GO
if exists (select * from sysobjects where id = object_id('dbo.Items') and sysstat & 0xf = 3)
	drop table "dbo"."Items"
GO
if exists (select * from sysobjects where id = object_id('dbo.Categories') and sysstat & 0xf = 3)
	drop table "dbo"."Categories"
GO


CREATE TABLE "Categories" (
	"CategoryID" "int" IDENTITY (1, 1) NOT NULL ,
	"CategoryName" nvarchar (15) NOT NULL ,
	"Description" "ntext" NULL ,
	CONSTRAINT "PK_Categories" PRIMARY KEY  CLUSTERED 
	(
		"CategoryID"
	)
)
GO

CREATE TABLE "Items" (
	"ItemID" "int" IDENTITY (1, 1) NOT NULL ,
	"ItemName" nvarchar (40) NOT NULL ,
	"CategoryID" "int" NULL ,
	CONSTRAINT "PK_Items" PRIMARY KEY  CLUSTERED 
	(
		"ItemID"
	),
	CONSTRAINT "FK_Items_Categories" FOREIGN KEY 
	(
		"CategoryID"
	) REFERENCES "dbo"."Categories" (
		"CategoryID"
	) ON DELETE CASCADE
)
GO
 CREATE  INDEX "CategoriesItems" ON "dbo"."Items"("CategoryID")
GO
 CREATE  INDEX "CategoryID" ON "dbo"."Items"("CategoryID")
GO
 CREATE  INDEX "ItemName" ON "dbo"."Items"("ItemName")
GO

set quoted_identifier on
go
set identity_insert "Categories" on
go
ALTER TABLE "Categories" NOCHECK CONSTRAINT ALL
go
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(1,'Beverages','Soft drinks, coffees, teas, beers, and ales')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(2,'Condiments','Sweet and savory sauces, relishes, spreads, and seasonings')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(3,'Confections','Desserts, candies, and sweet breads')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(4,'Dairy Products','Cheeses')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(5,'Grains/Cereals','Breads, crackers, pasta, and cereal')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(6,'Meat/Poultry','Prepared meats')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(7,'Produce','Dried fruit and bean curd')
INSERT "Categories"("CategoryID","CategoryName","Description") VALUES(8,'Seafood','Seaweed and fish')
go
set identity_insert "Categories" off
go
ALTER TABLE "Categories" CHECK CONSTRAINT ALL
go
set quoted_identifier on
go
set quoted_identifier on
go
set identity_insert "Items" on
go
ALTER TABLE "Items" NOCHECK CONSTRAINT ALL
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(1,'Chai',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(2,'Chang',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(3,'Aniseed Syrup',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(4,'Chef Anton''s Cajun Seasoning',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(5,'Chef Anton''s Gumbo Mix',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(6,'Grandma''s Boysenberry Spread',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(7,'Uncle Bob''s Organic Dried Pears',7)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(8,'Northwoods Cranberry Sauce',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(9,'Mishi Kobe Niku',6)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(10,'Ikura',8)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(11,'Queso Cabrales',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(12,'Queso Manchego La Pastora',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(13,'Konbu',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(14,'Tofu',7)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(15,'Genen Shouyu',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(16,'Pavlova',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(17,'Alice Mutton',6)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(18,'Carnarvon Tigers',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(19,'Teatime Chocolate Biscuits',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(20,'Sir Rodney''s Marmalade',3)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(21,'Sir Rodney''s Scones',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(22,'Gustaf''s Knäckebröd',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(23,'Tunnbröd',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(24,'Guaraná Fantástica',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(25,'NuNuCa Nuß-Nougat-Creme',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(26,'Gumbär Gummibärchen',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(27,'Schoggi Schokolade',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(28,'Rössle Sauerkraut',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(29,'Thüringer Rostbratwurst',6)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(30,'Nord-Ost Matjeshering',8)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(31,'Gorgonzola Telino',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(32,'Mascarpone Fabioli',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(33,'Geitost',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(34,'Sasquatch Ale',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(35,'Steeleye Stout',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(36,'Inlagd Sill',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(37,'Gravad lax',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(38,'Côte de Blaye',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(39,'Chartreuse verte',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(40,'Boston Crab Meat',8)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(41,'Jack''s New England Clam Chowder',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(42,'Singaporean Hokkien Fried Mee',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(43,'Ipoh Coffee',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(44,'Gula Malacca',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(45,'Rogede sild',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(46,'Spegesild',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(47,'Zaanse koeken',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(48,'Chocolade',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(49,'Maxilaku',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(50,'Valkoinen suklaa',3)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(51,'Manjimup Dried Apples',7)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(52,'Filo Mix',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(53,'Perth Pasties',6)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(54,'Tourtière',6)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(55,'Pâté chinois',6)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(56,'Gnocchi di nonna Alice',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(57,'Ravioli Angelo',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(58,'Escargots de Bourgogne',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(59,'Raclette Courdavault',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(60,'Camembert Pierrot',4)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(61,'Sirop d''érable',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(62,'Tarte au sucre',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(63,'Vegie-spread',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(64,'Wimmers gute Semmelknödel',5)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(65,'Louisiana Fiery Hot Pepper Sauce',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(66,'Louisiana Hot Spiced Okra',2)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(67,'Laughing Lumberjack Lager',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(68,'Scottish Longbreads',3)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(69,'Gudbrandsdalsost',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(70,'Outback Lager',1)
go
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(71,'Flotemysost',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(72,'Mozzarella di Giovanni',4)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(73,'Röd Kaviar',8)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(74,'Longlife Tofu',7)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(75,'Rhönbräu Klosterbier',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(76,'Lakkalikööri',1)
INSERT "Items"("ItemID","ItemName","CategoryID") VALUES(77,'Original Frankfurter grüne Soße',2)
go
set identity_insert "Items" off
go
ALTER TABLE "Items" CHECK CONSTRAINT ALL
go
set quoted_identifier on
go