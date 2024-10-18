using minimal_api;

IHostBuilder CreateHostBuilder(){
    return Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder =>{
            webBuilder.UseStartup<Startup>();
        });
}

CreateHostBuilder()
    .Build()
    .Run();