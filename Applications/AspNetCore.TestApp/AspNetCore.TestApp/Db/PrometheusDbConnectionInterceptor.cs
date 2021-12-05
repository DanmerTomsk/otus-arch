using Microsoft.EntityFrameworkCore.Diagnostics;
using Prometheus;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.TestApp.Db
{
    public class PrometheusDbConnectionInterceptor : DbConnectionInterceptor
    {
        private static readonly Counter DbExceptionsCounter = Metrics
            .CreateCounter("myapp_db_exceptions", "Counter of DB exceptions.");

        public override void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
        {
            DbExceptionsCounter.Inc();
            base.ConnectionFailed(connection, eventData);
        }

        public override Task ConnectionFailedAsync(DbConnection connection, ConnectionErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            DbExceptionsCounter.Inc();
            return base.ConnectionFailedAsync(connection, eventData, cancellationToken);
        }
    }
}
