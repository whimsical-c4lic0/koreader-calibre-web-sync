namespace Sync.Application.WebApp;

class IndexHandler
{
    public IResult Handle()
    {
        const string html = @"
            <!DOCTYPE html>
            <html lang=""en-US"">
            <head>
                <title>Title of the document</title>
                <link rel=""stylesheet"" type=""text/css"" href=""static/styles/main.css"">
                <script src=""static/js/main.js""></script>
            </head>

            <body>
            The content of the document......
            </body>

            </html>
        ";

        return Results.Content(html, "text/html", statusCode: 200);
    }
}
