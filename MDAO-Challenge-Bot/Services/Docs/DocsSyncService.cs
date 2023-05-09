using Common.Services;
using Google.Apis.Docs.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDAO_Challenge_Bot.Services.Docs;
public class DocsSyncService : Singleton
{
    [Inject]
    private readonly DocsService DocsService = null!;

    protected override ValueTask InitializeAsync()
    {
        var res = DocsService.Documents.Get("");
        return base.InitializeAsync();
    }

    protected override ValueTask RunAsync()
    {


        return base.RunAsync();
    }
}
