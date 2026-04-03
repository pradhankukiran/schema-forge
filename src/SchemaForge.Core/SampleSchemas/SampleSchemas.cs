using SchemaForge.Core.Models;

namespace SchemaForge.Core.SampleSchemas;

public static class SampleSchemas
{
    public static SchemaDocument Get(string key) => key switch
    {
        "blog" => BlogSchema(),
        "ecommerce" => ECommerceSchema(),
        "northwind" => NorthwindSchema(),
        _ => new SchemaDocument { Name = key }
    };

    private static string NewId() => Guid.NewGuid().ToString();

    private static SchemaDocument BlogSchema()
    {
        var usersId = NewId();
        var mediaAssetsId = NewId();
        var postsId = NewId();
        var postRevisionsId = NewId();
        var commentsId = NewId();
        var tagsId = NewId();
        var categoriesId = NewId();
        var postTagsId = NewId();
        var postCategoriesId = NewId();

        var usersIdCol = NewId();
        var mediaAssetsIdCol = NewId();
        var mediaUploaderCol = NewId();
        var postsIdCol = NewId();
        var postsAuthorCol = NewId();
        var postsFeaturedMediaCol = NewId();
        var postRevisionsIdCol = NewId();
        var postRevisionsPostCol = NewId();
        var postRevisionsEditorCol = NewId();
        var commentsIdCol = NewId();
        var commentsPostCol = NewId();
        var commentsAuthorCol = NewId();
        var commentsParentCol = NewId();
        var tagsIdCol = NewId();
        var categoriesIdCol = NewId();
        var categoriesParentCol = NewId();
        var postTagsPostCol = NewId();
        var postTagsTagCol = NewId();
        var postCategoriesPostCol = NewId();
        var postCategoriesCategoryCol = NewId();

        return new SchemaDocument
        {
            Name = "Blog",
            Description = "Editorial blog schema with revisions, media, categories, comments, and tags.",
            Tables =
            [
                new TableDefinition
                {
                    Id = usersId,
                    Name = "users",
                    AccentColor = "#1B5E9E",
                    Position = new CanvasPosition { X = 50, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = usersIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "username", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, IsUnique = true, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "email", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, IsUnique = true, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "display_name", DataType = LogicalDataType.Varchar, Length = 100, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "bio", DataType = LogicalDataType.Text, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 5 },
                    ]
                },
                new TableDefinition
                {
                    Id = mediaAssetsId,
                    Name = "media_assets",
                    AccentColor = "#00838F",
                    Position = new CanvasPosition { X = 50, Y = 310 },
                    Columns =
                    [
                        new ColumnDefinition { Id = mediaAssetsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = mediaUploaderCol, Name = "uploader_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "file_name", DataType = LogicalDataType.Varchar, Length = 180, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "storage_path", DataType = LogicalDataType.Varchar, Length = 255, IsNullable = false, IsUnique = true, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "mime_type", DataType = LogicalDataType.Varchar, Length = 80, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "size_bytes", DataType = LogicalDataType.BigInteger, IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = postsId,
                    Name = "posts",
                    AccentColor = "#2E7D32",
                    Position = new CanvasPosition { X = 370, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = postsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = postsAuthorCol, Name = "author_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = postsFeaturedMediaCol, Name = "featured_media_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "title", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "slug", DataType = LogicalDataType.Varchar, Length = 160, IsNullable = false, IsUnique = true, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "excerpt", DataType = LogicalDataType.Text, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "body", DataType = LogicalDataType.Text, OrdinalPosition = 6 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'draft'", IsNullable = false, OrdinalPosition = 7 },
                        new ColumnDefinition { Name = "published_at", DataType = LogicalDataType.DateTime, OrdinalPosition = 8 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 9 },
                    ]
                },
                new TableDefinition
                {
                    Id = postRevisionsId,
                    Name = "post_revisions",
                    AccentColor = "#6D4C41",
                    Position = new CanvasPosition { X = 370, Y = 350 },
                    Columns =
                    [
                        new ColumnDefinition { Id = postRevisionsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = postRevisionsPostCol, Name = "post_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = postRevisionsEditorCol, Name = "editor_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "revision_no", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "title_snapshot", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "body_snapshot", DataType = LogicalDataType.Text, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = commentsId,
                    Name = "comments",
                    AccentColor = "#EF6C00",
                    Position = new CanvasPosition { X = 370, Y = 660 },
                    Columns =
                    [
                        new ColumnDefinition { Id = commentsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = commentsPostCol, Name = "post_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = commentsAuthorCol, Name = "author_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Id = commentsParentCol, Name = "parent_comment_id", DataType = LogicalDataType.Integer, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "body", DataType = LogicalDataType.Text, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'visible'", IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = tagsId,
                    Name = "tags",
                    AccentColor = "#6A1B9A",
                    Position = new CanvasPosition { X = 730, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = tagsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, IsUnique = true, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "slug", DataType = LogicalDataType.Varchar, Length = 60, IsNullable = false, IsUnique = true, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = categoriesId,
                    Name = "categories",
                    AccentColor = "#7B1FA2",
                    Position = new CanvasPosition { X = 730, Y = 300 },
                    Columns =
                    [
                        new ColumnDefinition { Id = categoriesIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = categoriesParentCol, Name = "parent_category_id", DataType = LogicalDataType.Integer, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "slug", DataType = LogicalDataType.Varchar, Length = 120, IsNullable = false, IsUnique = true, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "sort_order", DataType = LogicalDataType.Integer, DefaultValue = "0", IsNullable = false, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = postTagsId,
                    Name = "post_tags",
                    AccentColor = "#37474F",
                    Position = new CanvasPosition { X = 1080, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = postTagsPostCol, Name = "post_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = postTagsTagCol, Name = "tag_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "assigned_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = postCategoriesId,
                    Name = "post_categories",
                    AccentColor = "#455A64",
                    Position = new CanvasPosition { X = 1080, Y = 300 },
                    Columns =
                    [
                        new ColumnDefinition { Id = postCategoriesPostCol, Name = "post_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = postCategoriesCategoryCol, Name = "category_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "is_primary", DataType = LogicalDataType.Boolean, DefaultValue = "0", IsNullable = false, OrdinalPosition = 2 },
                    ]
                },
            ],
            Relationships =
            [
                new RelationshipDefinition { Name = "fk_media_assets_uploader", SourceTableId = mediaAssetsId, SourceColumnId = mediaUploaderCol, TargetTableId = usersId, TargetColumnId = usersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_posts_author", SourceTableId = postsId, SourceColumnId = postsAuthorCol, TargetTableId = usersId, TargetColumnId = usersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_posts_featured_media", SourceTableId = postsId, SourceColumnId = postsFeaturedMediaCol, TargetTableId = mediaAssetsId, TargetColumnId = mediaAssetsIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_post_revisions_post", SourceTableId = postRevisionsId, SourceColumnId = postRevisionsPostCol, TargetTableId = postsId, TargetColumnId = postsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_post_revisions_editor", SourceTableId = postRevisionsId, SourceColumnId = postRevisionsEditorCol, TargetTableId = usersId, TargetColumnId = usersIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_comments_post", SourceTableId = commentsId, SourceColumnId = commentsPostCol, TargetTableId = postsId, TargetColumnId = postsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_comments_author", SourceTableId = commentsId, SourceColumnId = commentsAuthorCol, TargetTableId = usersId, TargetColumnId = usersIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_comments_parent", SourceTableId = commentsId, SourceColumnId = commentsParentCol, TargetTableId = commentsId, TargetColumnId = commentsIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_categories_parent", SourceTableId = categoriesId, SourceColumnId = categoriesParentCol, TargetTableId = categoriesId, TargetColumnId = categoriesIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_post_tags_post", SourceTableId = postTagsId, SourceColumnId = postTagsPostCol, TargetTableId = postsId, TargetColumnId = postsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_post_tags_tag", SourceTableId = postTagsId, SourceColumnId = postTagsTagCol, TargetTableId = tagsId, TargetColumnId = tagsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_post_categories_post", SourceTableId = postCategoriesId, SourceColumnId = postCategoriesPostCol, TargetTableId = postsId, TargetColumnId = postsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_post_categories_category", SourceTableId = postCategoriesId, SourceColumnId = postCategoriesCategoryCol, TargetTableId = categoriesId, TargetColumnId = categoriesIdCol, OnDelete = ReferentialAction.Cascade },
            ]
        };
    }

    private static SchemaDocument ECommerceSchema()
    {
        var customersId = NewId();
        var addressesId = NewId();
        var cartsId = NewId();
        var ordersId = NewId();
        var paymentsId = NewId();
        var shipmentsId = NewId();
        var categoriesId = NewId();
        var productsId = NewId();
        var orderItemsId = NewId();
        var reviewsId = NewId();
        var cartItemsId = NewId();

        var customersIdCol = NewId();
        var addressesIdCol = NewId();
        var addressesCustomerCol = NewId();
        var cartsIdCol = NewId();
        var cartsCustomerCol = NewId();
        var ordersIdCol = NewId();
        var ordersCustomerCol = NewId();
        var ordersAddressCol = NewId();
        var paymentsIdCol = NewId();
        var paymentsOrderCol = NewId();
        var shipmentsIdCol = NewId();
        var shipmentsOrderCol = NewId();
        var shipmentsAddressCol = NewId();
        var categoriesIdCol = NewId();
        var categoriesParentCol = NewId();
        var productsIdCol = NewId();
        var productsCategoryCol = NewId();
        var orderItemsOrderCol = NewId();
        var orderItemsProductCol = NewId();
        var reviewsIdCol = NewId();
        var reviewsProductCol = NewId();
        var reviewsCustomerCol = NewId();
        var cartItemsCartCol = NewId();
        var cartItemsProductCol = NewId();

        return new SchemaDocument
        {
            Name = "E-Commerce",
            Description = "Commerce workflow with carts, orders, payments, shipments, and reviews.",
            Tables =
            [
                new TableDefinition
                {
                    Id = customersId,
                    Name = "customers",
                    AccentColor = "#1B5E9E",
                    Position = new CanvasPosition { X = 50, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = customersIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "email", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, IsUnique = true, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "first_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "last_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "loyalty_tier", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'standard'", IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = addressesId,
                    Name = "addresses",
                    AccentColor = "#37474F",
                    Position = new CanvasPosition { X = 50, Y = 340 },
                    Columns =
                    [
                        new ColumnDefinition { Id = addressesIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = addressesCustomerCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "label", DataType = LogicalDataType.Varchar, Length = 40, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "line1", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "line2", DataType = LogicalDataType.Varchar, Length = 200, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "city", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "postal_code", DataType = LogicalDataType.Varchar, Length = 20, IsNullable = false, OrdinalPosition = 6 },
                        new ColumnDefinition { Name = "country", DataType = LogicalDataType.Varchar, Length = 2, IsNullable = false, OrdinalPosition = 7 },
                        new ColumnDefinition { Name = "is_default", DataType = LogicalDataType.Boolean, DefaultValue = "0", IsNullable = false, OrdinalPosition = 8 },
                    ]
                },
                new TableDefinition
                {
                    Id = cartsId,
                    Name = "carts",
                    AccentColor = "#8D6E63",
                    Position = new CanvasPosition { X = 50, Y = 690 },
                    Columns =
                    [
                        new ColumnDefinition { Id = cartsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = cartsCustomerCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'active'", IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "updated_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = ordersId,
                    Name = "orders",
                    AccentColor = "#EF6C00",
                    Position = new CanvasPosition { X = 390, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = ordersIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = ordersCustomerCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = ordersAddressCol, Name = "shipping_address_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'pending'", IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "total", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "placed_at", DataType = LogicalDataType.DateTime, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = paymentsId,
                    Name = "payments",
                    AccentColor = "#C62828",
                    Position = new CanvasPosition { X = 390, Y = 360 },
                    Columns =
                    [
                        new ColumnDefinition { Id = paymentsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = paymentsOrderCol, Name = "order_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "provider", DataType = LogicalDataType.Varchar, Length = 40, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "provider_ref", DataType = LogicalDataType.Varchar, Length = 80, IsUnique = true, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "amount", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'authorized'", IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "paid_at", DataType = LogicalDataType.DateTime, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = shipmentsId,
                    Name = "shipments",
                    AccentColor = "#5D4037",
                    Position = new CanvasPosition { X = 390, Y = 650 },
                    Columns =
                    [
                        new ColumnDefinition { Id = shipmentsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = shipmentsOrderCol, Name = "order_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = shipmentsAddressCol, Name = "address_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "carrier", DataType = LogicalDataType.Varchar, Length = 60, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "tracking_number", DataType = LogicalDataType.Varchar, Length = 80, IsUnique = true, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'queued'", IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "shipped_at", DataType = LogicalDataType.DateTime, OrdinalPosition = 6 },
                        new ColumnDefinition { Name = "delivered_at", DataType = LogicalDataType.DateTime, OrdinalPosition = 7 },
                    ]
                },
                new TableDefinition
                {
                    Id = categoriesId,
                    Name = "categories",
                    AccentColor = "#6A1B9A",
                    Position = new CanvasPosition { X = 760, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = categoriesIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = categoriesParentCol, Name = "parent_category_id", DataType = LogicalDataType.Integer, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "slug", DataType = LogicalDataType.Varchar, Length = 120, IsNullable = false, IsUnique = true, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "description", DataType = LogicalDataType.Text, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = productsId,
                    Name = "products",
                    AccentColor = "#2E7D32",
                    Position = new CanvasPosition { X = 760, Y = 350 },
                    Columns =
                    [
                        new ColumnDefinition { Id = productsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = productsCategoryCol, Name = "category_id", DataType = LogicalDataType.Integer, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "sku", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, IsUnique = true, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "description", DataType = LogicalDataType.Text, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "stock_qty", DataType = LogicalDataType.Integer, DefaultValue = "0", IsNullable = false, OrdinalPosition = 6 },
                        new ColumnDefinition { Name = "is_active", DataType = LogicalDataType.Boolean, DefaultValue = "1", IsNullable = false, OrdinalPosition = 7 },
                    ]
                },
                new TableDefinition
                {
                    Id = orderItemsId,
                    Name = "order_items",
                    AccentColor = "#AD1457",
                    Position = new CanvasPosition { X = 1110, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = orderItemsOrderCol, Name = "order_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = orderItemsProductCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "quantity", DataType = LogicalDataType.Integer, IsNullable = false, DefaultValue = "1", OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "unit_price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "discount", DataType = LogicalDataType.Decimal, Precision = 5, Scale = 2, DefaultValue = "0", IsNullable = false, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = reviewsId,
                    Name = "reviews",
                    AccentColor = "#00838F",
                    Position = new CanvasPosition { X = 1110, Y = 350 },
                    Columns =
                    [
                        new ColumnDefinition { Id = reviewsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = reviewsProductCol, Name = "product_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = reviewsCustomerCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "rating", DataType = LogicalDataType.Integer, IsNullable = false, CheckExpression = "rating BETWEEN 1 AND 5", OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "title", DataType = LogicalDataType.Varchar, Length = 120, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "body", DataType = LogicalDataType.Text, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = cartItemsId,
                    Name = "cart_items",
                    AccentColor = "#455A64",
                    Position = new CanvasPosition { X = 1110, Y = 650 },
                    Columns =
                    [
                        new ColumnDefinition { Id = cartItemsCartCol, Name = "cart_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = cartItemsProductCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "quantity", DataType = LogicalDataType.Integer, IsNullable = false, DefaultValue = "1", OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "added_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 3 },
                    ]
                },
            ],
            Relationships =
            [
                new RelationshipDefinition { Name = "fk_addresses_customer", SourceTableId = addressesId, SourceColumnId = addressesCustomerCol, TargetTableId = customersId, TargetColumnId = customersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_carts_customer", SourceTableId = cartsId, SourceColumnId = cartsCustomerCol, TargetTableId = customersId, TargetColumnId = customersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_orders_customer", SourceTableId = ordersId, SourceColumnId = ordersCustomerCol, TargetTableId = customersId, TargetColumnId = customersIdCol },
                new RelationshipDefinition { Name = "fk_orders_address", SourceTableId = ordersId, SourceColumnId = ordersAddressCol, TargetTableId = addressesId, TargetColumnId = addressesIdCol },
                new RelationshipDefinition { Name = "fk_payments_order", SourceTableId = paymentsId, SourceColumnId = paymentsOrderCol, TargetTableId = ordersId, TargetColumnId = ordersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_shipments_order", SourceTableId = shipmentsId, SourceColumnId = shipmentsOrderCol, TargetTableId = ordersId, TargetColumnId = ordersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_shipments_address", SourceTableId = shipmentsId, SourceColumnId = shipmentsAddressCol, TargetTableId = addressesId, TargetColumnId = addressesIdCol },
                new RelationshipDefinition { Name = "fk_categories_parent", SourceTableId = categoriesId, SourceColumnId = categoriesParentCol, TargetTableId = categoriesId, TargetColumnId = categoriesIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_products_category", SourceTableId = productsId, SourceColumnId = productsCategoryCol, TargetTableId = categoriesId, TargetColumnId = categoriesIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_order_items_order", SourceTableId = orderItemsId, SourceColumnId = orderItemsOrderCol, TargetTableId = ordersId, TargetColumnId = ordersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_order_items_product", SourceTableId = orderItemsId, SourceColumnId = orderItemsProductCol, TargetTableId = productsId, TargetColumnId = productsIdCol },
                new RelationshipDefinition { Name = "fk_reviews_product", SourceTableId = reviewsId, SourceColumnId = reviewsProductCol, TargetTableId = productsId, TargetColumnId = productsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_reviews_customer", SourceTableId = reviewsId, SourceColumnId = reviewsCustomerCol, TargetTableId = customersId, TargetColumnId = customersIdCol },
                new RelationshipDefinition { Name = "fk_cart_items_cart", SourceTableId = cartItemsId, SourceColumnId = cartItemsCartCol, TargetTableId = cartsId, TargetColumnId = cartsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_cart_items_product", SourceTableId = cartItemsId, SourceColumnId = cartItemsProductCol, TargetTableId = productsId, TargetColumnId = productsIdCol, OnDelete = ReferentialAction.Cascade },
            ]
        };
    }

    private static SchemaDocument NorthwindSchema()
    {
        var customersId = NewId();
        var customerDemographicsId = NewId();
        var customerCustomerDemoId = NewId();
        var employeesId = NewId();
        var regionsId = NewId();
        var territoriesId = NewId();
        var employeeTerritoriesId = NewId();
        var ordersId = NewId();
        var orderDetailsId = NewId();
        var categoriesId = NewId();
        var suppliersId = NewId();
        var productsId = NewId();
        var shippersId = NewId();

        var customersIdCol = NewId();
        var customerDemographicsIdCol = NewId();
        var customerCustomerDemoCustomerCol = NewId();
        var customerCustomerDemoTypeCol = NewId();
        var employeesIdCol = NewId();
        var employeesReportsToCol = NewId();
        var regionsIdCol = NewId();
        var territoriesIdCol = NewId();
        var territoriesRegionCol = NewId();
        var employeeTerritoriesEmployeeCol = NewId();
        var employeeTerritoriesTerritoryCol = NewId();
        var ordersIdCol = NewId();
        var ordersCustomerCol = NewId();
        var ordersEmployeeCol = NewId();
        var ordersShipperCol = NewId();
        var orderDetailsOrderCol = NewId();
        var orderDetailsProductCol = NewId();
        var categoriesIdCol = NewId();
        var suppliersIdCol = NewId();
        var productsIdCol = NewId();
        var productsCategoryCol = NewId();
        var productsSupplierCol = NewId();
        var shippersIdCol = NewId();

        return new SchemaDocument
        {
            Name = "Northwind",
            Description = "Expanded Northwind sample with regions, territories, and customer demographics.",
            Tables =
            [
                new TableDefinition
                {
                    Id = customersId,
                    Name = "customers",
                    AccentColor = "#1B5E9E",
                    Position = new CanvasPosition { X = 50, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = customersIdCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "company_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "contact_name", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "contact_title", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "city", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "country", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = customerDemographicsId,
                    Name = "customer_demographics",
                    AccentColor = "#6A1B9A",
                    Position = new CanvasPosition { X = 50, Y = 320 },
                    Columns =
                    [
                        new ColumnDefinition { Id = customerDemographicsIdCol, Name = "customer_type_id", DataType = LogicalDataType.Varchar, Length = 10, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "customer_desc", DataType = LogicalDataType.Text, OrdinalPosition = 1 },
                    ]
                },
                new TableDefinition
                {
                    Id = employeesId,
                    Name = "employees",
                    AccentColor = "#AD1457",
                    Position = new CanvasPosition { X = 50, Y = 560 },
                    Columns =
                    [
                        new ColumnDefinition { Id = employeesIdCol, Name = "employee_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = employeesReportsToCol, Name = "reports_to", DataType = LogicalDataType.Integer, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "first_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "last_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "title", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "city", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "hire_date", DataType = LogicalDataType.Date, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = regionsId,
                    Name = "regions",
                    AccentColor = "#5D4037",
                    Position = new CanvasPosition { X = 390, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = regionsIdCol, Name = "region_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "region_description", DataType = LogicalDataType.Varchar, Length = 60, IsNullable = false, OrdinalPosition = 1 },
                    ]
                },
                new TableDefinition
                {
                    Id = territoriesId,
                    Name = "territories",
                    AccentColor = "#8D6E63",
                    Position = new CanvasPosition { X = 390, Y = 260 },
                    Columns =
                    [
                        new ColumnDefinition { Id = territoriesIdCol, Name = "territory_id", DataType = LogicalDataType.Varchar, Length = 20, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = territoriesRegionCol, Name = "region_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "territory_description", DataType = LogicalDataType.Varchar, Length = 80, IsNullable = false, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = employeeTerritoriesId,
                    Name = "employee_territories",
                    AccentColor = "#455A64",
                    Position = new CanvasPosition { X = 390, Y = 500 },
                    Columns =
                    [
                        new ColumnDefinition { Id = employeeTerritoriesEmployeeCol, Name = "employee_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = employeeTerritoriesTerritoryCol, Name = "territory_id", DataType = LogicalDataType.Varchar, Length = 20, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "assigned_at", DataType = LogicalDataType.Date, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = ordersId,
                    Name = "orders",
                    AccentColor = "#EF6C00",
                    Position = new CanvasPosition { X = 730, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = ordersIdCol, Name = "order_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = ordersCustomerCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = ordersEmployeeCol, Name = "employee_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Id = ordersShipperCol, Name = "shipper_id", DataType = LogicalDataType.Integer, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "order_date", DataType = LogicalDataType.Date, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "required_date", DataType = LogicalDataType.Date, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "shipped_date", DataType = LogicalDataType.Date, OrdinalPosition = 6 },
                        new ColumnDefinition { Name = "freight", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, OrdinalPosition = 7 },
                    ]
                },
                new TableDefinition
                {
                    Id = orderDetailsId,
                    Name = "order_details",
                    AccentColor = "#C62828",
                    Position = new CanvasPosition { X = 730, Y = 320 },
                    Columns =
                    [
                        new ColumnDefinition { Id = orderDetailsOrderCol, Name = "order_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = orderDetailsProductCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "unit_price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "quantity", DataType = LogicalDataType.Integer, IsNullable = false, DefaultValue = "1", OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "discount", DataType = LogicalDataType.Float, DefaultValue = "0", OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = customerCustomerDemoId,
                    Name = "customer_customer_demo",
                    AccentColor = "#546E7A",
                    Position = new CanvasPosition { X = 730, Y = 580 },
                    Columns =
                    [
                        new ColumnDefinition { Id = customerCustomerDemoCustomerCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = customerCustomerDemoTypeCol, Name = "customer_type_id", DataType = LogicalDataType.Varchar, Length = 10, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "assigned_at", DataType = LogicalDataType.Date, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = shippersId,
                    Name = "shippers",
                    AccentColor = "#37474F",
                    Position = new CanvasPosition { X = 1090, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = shippersIdCol, Name = "shipper_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "company_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = categoriesId,
                    Name = "categories",
                    AccentColor = "#7B1FA2",
                    Position = new CanvasPosition { X = 1090, Y = 260 },
                    Columns =
                    [
                        new ColumnDefinition { Id = categoriesIdCol, Name = "category_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "category_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "description", DataType = LogicalDataType.Text, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = suppliersId,
                    Name = "suppliers",
                    AccentColor = "#00838F",
                    Position = new CanvasPosition { X = 1090, Y = 500 },
                    Columns =
                    [
                        new ColumnDefinition { Id = suppliersIdCol, Name = "supplier_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "company_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "contact_name", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "country", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = productsId,
                    Name = "products",
                    AccentColor = "#2E7D32",
                    Position = new CanvasPosition { X = 1090, Y = 760 },
                    Columns =
                    [
                        new ColumnDefinition { Id = productsIdCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "product_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = productsCategoryCol, Name = "category_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Id = productsSupplierCol, Name = "supplier_id", DataType = LogicalDataType.Integer, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "quantity_per_unit", DataType = LogicalDataType.Varchar, Length = 40, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "unit_price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "units_in_stock", DataType = LogicalDataType.Integer, DefaultValue = "0", OrdinalPosition = 6 },
                        new ColumnDefinition { Name = "discontinued", DataType = LogicalDataType.Boolean, DefaultValue = "0", IsNullable = false, OrdinalPosition = 7 },
                    ]
                },
            ],
            Relationships =
            [
                new RelationshipDefinition { Name = "fk_customer_customer_demo_customer", SourceTableId = customerCustomerDemoId, SourceColumnId = customerCustomerDemoCustomerCol, TargetTableId = customersId, TargetColumnId = customersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_customer_customer_demo_type", SourceTableId = customerCustomerDemoId, SourceColumnId = customerCustomerDemoTypeCol, TargetTableId = customerDemographicsId, TargetColumnId = customerDemographicsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_employees_reports_to", SourceTableId = employeesId, SourceColumnId = employeesReportsToCol, TargetTableId = employeesId, TargetColumnId = employeesIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_territories_region", SourceTableId = territoriesId, SourceColumnId = territoriesRegionCol, TargetTableId = regionsId, TargetColumnId = regionsIdCol },
                new RelationshipDefinition { Name = "fk_employee_territories_employee", SourceTableId = employeeTerritoriesId, SourceColumnId = employeeTerritoriesEmployeeCol, TargetTableId = employeesId, TargetColumnId = employeesIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_employee_territories_territory", SourceTableId = employeeTerritoriesId, SourceColumnId = employeeTerritoriesTerritoryCol, TargetTableId = territoriesId, TargetColumnId = territoriesIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_orders_customer", SourceTableId = ordersId, SourceColumnId = ordersCustomerCol, TargetTableId = customersId, TargetColumnId = customersIdCol },
                new RelationshipDefinition { Name = "fk_orders_employee", SourceTableId = ordersId, SourceColumnId = ordersEmployeeCol, TargetTableId = employeesId, TargetColumnId = employeesIdCol },
                new RelationshipDefinition { Name = "fk_orders_shipper", SourceTableId = ordersId, SourceColumnId = ordersShipperCol, TargetTableId = shippersId, TargetColumnId = shippersIdCol },
                new RelationshipDefinition { Name = "fk_order_details_order", SourceTableId = orderDetailsId, SourceColumnId = orderDetailsOrderCol, TargetTableId = ordersId, TargetColumnId = ordersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_order_details_product", SourceTableId = orderDetailsId, SourceColumnId = orderDetailsProductCol, TargetTableId = productsId, TargetColumnId = productsIdCol },
                new RelationshipDefinition { Name = "fk_products_category", SourceTableId = productsId, SourceColumnId = productsCategoryCol, TargetTableId = categoriesId, TargetColumnId = categoriesIdCol },
                new RelationshipDefinition { Name = "fk_products_supplier", SourceTableId = productsId, SourceColumnId = productsSupplierCol, TargetTableId = suppliersId, TargetColumnId = suppliersIdCol },
            ]
        };
    }
}
