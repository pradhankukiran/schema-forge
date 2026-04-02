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

    private static SchemaDocument BlogSchema()
    {
        var usersId = Guid.NewGuid().ToString();
        var postsId = Guid.NewGuid().ToString();
        var commentsId = Guid.NewGuid().ToString();
        var tagsId = Guid.NewGuid().ToString();
        var postTagsId = Guid.NewGuid().ToString();

        var usersIdCol = Guid.NewGuid().ToString();
        var postsIdCol = Guid.NewGuid().ToString();
        var postsAuthorCol = Guid.NewGuid().ToString();
        var commentsIdCol = Guid.NewGuid().ToString();
        var commentsPostCol = Guid.NewGuid().ToString();
        var commentsAuthorCol = Guid.NewGuid().ToString();
        var tagsIdCol = Guid.NewGuid().ToString();
        var ptPostCol = Guid.NewGuid().ToString();
        var ptTagCol = Guid.NewGuid().ToString();

        return new SchemaDocument
        {
            Name = "Blog",
            Description = "A simple blog schema with users, posts, comments, and tags.",
            Tables =
            [
                new TableDefinition
                {
                    Id = usersId, Name = "users", AccentColor = "#1B5E9E",
                    Position = new CanvasPosition { X = 50, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = usersIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "username", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, IsUnique = true, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "email", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, IsUnique = true, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "display_name", DataType = LogicalDataType.Varchar, Length = 100, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = postsId, Name = "posts", AccentColor = "#2E7D32",
                    Position = new CanvasPosition { X = 350, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = postsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = postsAuthorCol, Name = "author_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "title", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "body", DataType = LogicalDataType.Text, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'draft'", OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "published_at", DataType = LogicalDataType.DateTime, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = commentsId, Name = "comments", AccentColor = "#EF6C00",
                    Position = new CanvasPosition { X = 350, Y = 300 },
                    Columns =
                    [
                        new ColumnDefinition { Id = commentsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = commentsPostCol, Name = "post_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = commentsAuthorCol, Name = "author_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "body", DataType = LogicalDataType.Text, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = tagsId, Name = "tags", AccentColor = "#6A1B9A",
                    Position = new CanvasPosition { X = 700, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = tagsIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, IsUnique = true, OrdinalPosition = 1 },
                    ]
                },
                new TableDefinition
                {
                    Id = postTagsId, Name = "post_tags", AccentColor = "#37474F",
                    Position = new CanvasPosition { X = 700, Y = 200 },
                    Columns =
                    [
                        new ColumnDefinition { Id = ptPostCol, Name = "post_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = ptTagCol, Name = "tag_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                    ]
                },
            ],
            Relationships =
            [
                new RelationshipDefinition { Name = "fk_posts_author", SourceTableId = postsId, SourceColumnId = postsAuthorCol, TargetTableId = usersId, TargetColumnId = usersIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_comments_post", SourceTableId = commentsId, SourceColumnId = commentsPostCol, TargetTableId = postsId, TargetColumnId = postsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_comments_author", SourceTableId = commentsId, SourceColumnId = commentsAuthorCol, TargetTableId = usersId, TargetColumnId = usersIdCol },
                new RelationshipDefinition { Name = "fk_post_tags_post", SourceTableId = postTagsId, SourceColumnId = ptPostCol, TargetTableId = postsId, TargetColumnId = postsIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_post_tags_tag", SourceTableId = postTagsId, SourceColumnId = ptTagCol, TargetTableId = tagsId, TargetColumnId = tagsIdCol, OnDelete = ReferentialAction.Cascade },
            ]
        };
    }

    private static SchemaDocument ECommerceSchema()
    {
        var customersId = Guid.NewGuid().ToString();
        var productsId = Guid.NewGuid().ToString();
        var categoriesId = Guid.NewGuid().ToString();
        var ordersId = Guid.NewGuid().ToString();
        var orderItemsId = Guid.NewGuid().ToString();
        var reviewsId = Guid.NewGuid().ToString();
        var addressesId = Guid.NewGuid().ToString();

        var custIdCol = Guid.NewGuid().ToString();
        var prodIdCol = Guid.NewGuid().ToString();
        var prodCatCol = Guid.NewGuid().ToString();
        var catIdCol = Guid.NewGuid().ToString();
        var ordIdCol = Guid.NewGuid().ToString();
        var ordCustCol = Guid.NewGuid().ToString();
        var ordAddrCol = Guid.NewGuid().ToString();
        var oiOrdCol = Guid.NewGuid().ToString();
        var oiProdCol = Guid.NewGuid().ToString();
        var revProdCol = Guid.NewGuid().ToString();
        var revCustCol = Guid.NewGuid().ToString();
        var addrIdCol = Guid.NewGuid().ToString();
        var addrCustCol = Guid.NewGuid().ToString();

        return new SchemaDocument
        {
            Name = "E-Commerce",
            Description = "Products, orders, customers, reviews.",
            Tables =
            [
                new TableDefinition
                {
                    Id = customersId, Name = "customers", AccentColor = "#1B5E9E",
                    Position = new CanvasPosition { X = 50, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = custIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "email", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, IsUnique = true, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "first_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "last_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = categoriesId, Name = "categories", AccentColor = "#6A1B9A",
                    Position = new CanvasPosition { X = 700, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = catIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "description", DataType = LogicalDataType.Text, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = productsId, Name = "products", AccentColor = "#2E7D32",
                    Position = new CanvasPosition { X = 400, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = prodIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = prodCatCol, Name = "category_id", DataType = LogicalDataType.Integer, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "name", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "description", DataType = LogicalDataType.Text, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "stock_qty", DataType = LogicalDataType.Integer, DefaultValue = "0", IsNullable = false, OrdinalPosition = 5 },
                    ]
                },
                new TableDefinition
                {
                    Id = ordersId, Name = "orders", AccentColor = "#EF6C00",
                    Position = new CanvasPosition { X = 50, Y = 280 },
                    Columns =
                    [
                        new ColumnDefinition { Id = ordIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = ordCustCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = ordAddrCol, Name = "shipping_address_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "status", DataType = LogicalDataType.Varchar, Length = 20, DefaultValue = "'pending'", IsNullable = false, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "total", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 5 },
                    ]
                },
                new TableDefinition
                {
                    Id = orderItemsId, Name = "order_items", AccentColor = "#C62828",
                    Position = new CanvasPosition { X = 400, Y = 280 },
                    Columns =
                    [
                        new ColumnDefinition { Id = oiOrdCol, Name = "order_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = oiProdCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "quantity", DataType = LogicalDataType.Integer, IsNullable = false, DefaultValue = "1", OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "unit_price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 3 },
                    ]
                },
                new TableDefinition
                {
                    Id = reviewsId, Name = "reviews", AccentColor = "#00838F",
                    Position = new CanvasPosition { X = 700, Y = 280 },
                    Columns =
                    [
                        new ColumnDefinition { Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = revProdCol, Name = "product_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = revCustCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "rating", DataType = LogicalDataType.Integer, IsNullable = false, CheckExpression = "rating BETWEEN 1 AND 5", OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "body", DataType = LogicalDataType.Text, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "created_at", DataType = LogicalDataType.DateTime, IsNullable = false, OrdinalPosition = 5 },
                    ]
                },
                new TableDefinition
                {
                    Id = addressesId, Name = "addresses", AccentColor = "#37474F",
                    Position = new CanvasPosition { X = 50, Y = 500 },
                    Columns =
                    [
                        new ColumnDefinition { Id = addrIdCol, Name = "id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = addrCustCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "line1", DataType = LogicalDataType.Varchar, Length = 200, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "line2", DataType = LogicalDataType.Varchar, Length = 200, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "city", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "postal_code", DataType = LogicalDataType.Varchar, Length = 20, IsNullable = false, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "country", DataType = LogicalDataType.Varchar, Length = 2, IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
            ],
            Relationships =
            [
                new RelationshipDefinition { Name = "fk_products_category", SourceTableId = productsId, SourceColumnId = prodCatCol, TargetTableId = categoriesId, TargetColumnId = catIdCol, OnDelete = ReferentialAction.SetNull },
                new RelationshipDefinition { Name = "fk_orders_customer", SourceTableId = ordersId, SourceColumnId = ordCustCol, TargetTableId = customersId, TargetColumnId = custIdCol },
                new RelationshipDefinition { Name = "fk_orders_address", SourceTableId = ordersId, SourceColumnId = ordAddrCol, TargetTableId = addressesId, TargetColumnId = addrIdCol },
                new RelationshipDefinition { Name = "fk_order_items_order", SourceTableId = orderItemsId, SourceColumnId = oiOrdCol, TargetTableId = ordersId, TargetColumnId = ordIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_order_items_product", SourceTableId = orderItemsId, SourceColumnId = oiProdCol, TargetTableId = productsId, TargetColumnId = prodIdCol },
                new RelationshipDefinition { Name = "fk_reviews_product", SourceTableId = reviewsId, SourceColumnId = revProdCol, TargetTableId = productsId, TargetColumnId = prodIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_reviews_customer", SourceTableId = reviewsId, SourceColumnId = revCustCol, TargetTableId = customersId, TargetColumnId = custIdCol },
                new RelationshipDefinition { Name = "fk_addresses_customer", SourceTableId = addressesId, SourceColumnId = addrCustCol, TargetTableId = customersId, TargetColumnId = custIdCol, OnDelete = ReferentialAction.Cascade },
            ]
        };
    }

    private static SchemaDocument NorthwindSchema()
    {
        var customersId = Guid.NewGuid().ToString();
        var ordersId = Guid.NewGuid().ToString();
        var orderDetailsId = Guid.NewGuid().ToString();
        var productsId = Guid.NewGuid().ToString();
        var categoriesId = Guid.NewGuid().ToString();
        var suppliersId = Guid.NewGuid().ToString();
        var employeesId = Guid.NewGuid().ToString();
        var shippersId = Guid.NewGuid().ToString();

        var custIdCol = Guid.NewGuid().ToString();
        var ordIdCol = Guid.NewGuid().ToString();
        var ordCustCol = Guid.NewGuid().ToString();
        var ordEmpCol = Guid.NewGuid().ToString();
        var ordShipCol = Guid.NewGuid().ToString();
        var odOrdCol = Guid.NewGuid().ToString();
        var odProdCol = Guid.NewGuid().ToString();
        var prodIdCol = Guid.NewGuid().ToString();
        var prodCatCol = Guid.NewGuid().ToString();
        var prodSupCol = Guid.NewGuid().ToString();
        var catIdCol = Guid.NewGuid().ToString();
        var supIdCol = Guid.NewGuid().ToString();
        var empIdCol = Guid.NewGuid().ToString();
        var shipIdCol = Guid.NewGuid().ToString();

        return new SchemaDocument
        {
            Name = "Northwind",
            Description = "Classic Northwind sample database.",
            Tables =
            [
                new TableDefinition
                {
                    Id = customersId, Name = "customers", AccentColor = "#1B5E9E",
                    Position = new CanvasPosition { X = 50, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = custIdCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "company_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "contact_name", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "city", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "country", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 5 },
                    ]
                },
                new TableDefinition
                {
                    Id = employeesId, Name = "employees", AccentColor = "#AD1457",
                    Position = new CanvasPosition { X = 50, Y = 300 },
                    Columns =
                    [
                        new ColumnDefinition { Id = empIdCol, Name = "employee_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "first_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "last_name", DataType = LogicalDataType.Varchar, Length = 50, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "title", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "hire_date", DataType = LogicalDataType.Date, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = shippersId, Name = "shippers", AccentColor = "#37474F",
                    Position = new CanvasPosition { X = 50, Y = 500 },
                    Columns =
                    [
                        new ColumnDefinition { Id = shipIdCol, Name = "shipper_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "company_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = ordersId, Name = "orders", AccentColor = "#EF6C00",
                    Position = new CanvasPosition { X = 370, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = ordIdCol, Name = "order_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = ordCustCol, Name = "customer_id", DataType = LogicalDataType.Integer, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = ordEmpCol, Name = "employee_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Id = ordShipCol, Name = "shipper_id", DataType = LogicalDataType.Integer, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "order_date", DataType = LogicalDataType.Date, IsNullable = false, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "shipped_date", DataType = LogicalDataType.Date, OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "freight", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, OrdinalPosition = 6 },
                    ]
                },
                new TableDefinition
                {
                    Id = orderDetailsId, Name = "order_details", AccentColor = "#C62828",
                    Position = new CanvasPosition { X = 370, Y = 300 },
                    Columns =
                    [
                        new ColumnDefinition { Id = odOrdCol, Name = "order_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Id = odProdCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "unit_price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, IsNullable = false, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "quantity", DataType = LogicalDataType.Integer, IsNullable = false, DefaultValue = "1", OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "discount", DataType = LogicalDataType.Float, DefaultValue = "0", OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = categoriesId, Name = "categories", AccentColor = "#6A1B9A",
                    Position = new CanvasPosition { X = 700, Y = 50 },
                    Columns =
                    [
                        new ColumnDefinition { Id = catIdCol, Name = "category_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "category_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "description", DataType = LogicalDataType.Text, OrdinalPosition = 2 },
                    ]
                },
                new TableDefinition
                {
                    Id = suppliersId, Name = "suppliers", AccentColor = "#00838F",
                    Position = new CanvasPosition { X = 700, Y = 250 },
                    Columns =
                    [
                        new ColumnDefinition { Id = supIdCol, Name = "supplier_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "company_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Name = "contact_name", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 2 },
                        new ColumnDefinition { Name = "phone", DataType = LogicalDataType.Varchar, Length = 24, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "country", DataType = LogicalDataType.Varchar, Length = 50, OrdinalPosition = 4 },
                    ]
                },
                new TableDefinition
                {
                    Id = productsId, Name = "products", AccentColor = "#2E7D32",
                    Position = new CanvasPosition { X = 700, Y = 430 },
                    Columns =
                    [
                        new ColumnDefinition { Id = prodIdCol, Name = "product_id", DataType = LogicalDataType.Integer, IsPrimaryKey = true, IsAutoIncrement = true, IsNullable = false, OrdinalPosition = 0 },
                        new ColumnDefinition { Name = "product_name", DataType = LogicalDataType.Varchar, Length = 100, IsNullable = false, OrdinalPosition = 1 },
                        new ColumnDefinition { Id = prodCatCol, Name = "category_id", DataType = LogicalDataType.Integer, OrdinalPosition = 2 },
                        new ColumnDefinition { Id = prodSupCol, Name = "supplier_id", DataType = LogicalDataType.Integer, OrdinalPosition = 3 },
                        new ColumnDefinition { Name = "unit_price", DataType = LogicalDataType.Decimal, Precision = 10, Scale = 2, OrdinalPosition = 4 },
                        new ColumnDefinition { Name = "units_in_stock", DataType = LogicalDataType.Integer, DefaultValue = "0", OrdinalPosition = 5 },
                        new ColumnDefinition { Name = "discontinued", DataType = LogicalDataType.Boolean, DefaultValue = "0", IsNullable = false, OrdinalPosition = 6 },
                    ]
                },
            ],
            Relationships =
            [
                new RelationshipDefinition { Name = "fk_orders_customer", SourceTableId = ordersId, SourceColumnId = ordCustCol, TargetTableId = customersId, TargetColumnId = custIdCol },
                new RelationshipDefinition { Name = "fk_orders_employee", SourceTableId = ordersId, SourceColumnId = ordEmpCol, TargetTableId = employeesId, TargetColumnId = empIdCol },
                new RelationshipDefinition { Name = "fk_orders_shipper", SourceTableId = ordersId, SourceColumnId = ordShipCol, TargetTableId = shippersId, TargetColumnId = shipIdCol },
                new RelationshipDefinition { Name = "fk_order_details_order", SourceTableId = orderDetailsId, SourceColumnId = odOrdCol, TargetTableId = ordersId, TargetColumnId = ordIdCol, OnDelete = ReferentialAction.Cascade },
                new RelationshipDefinition { Name = "fk_order_details_product", SourceTableId = orderDetailsId, SourceColumnId = odProdCol, TargetTableId = productsId, TargetColumnId = prodIdCol },
                new RelationshipDefinition { Name = "fk_products_category", SourceTableId = productsId, SourceColumnId = prodCatCol, TargetTableId = categoriesId, TargetColumnId = catIdCol },
                new RelationshipDefinition { Name = "fk_products_supplier", SourceTableId = productsId, SourceColumnId = prodSupCol, TargetTableId = suppliersId, TargetColumnId = supIdCol },
            ]
        };
    }
}
