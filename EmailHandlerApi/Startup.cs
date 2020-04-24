using System.Net;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;


namespace EmailHandlerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOptions();
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var host = config.GetValue<string>("EmailConfiguration:MailServerAddress");
                var port = config.GetValue<string>("EmailConfiguration:MailServerPort");
                var r = new NetworkCredential(config.GetValue<string>("EmailConfiguration:UserId"), config.GetValue<string>("EmailConfiguration:UserPassword"));

                var smtp = new SmtpClient()
                {

                };

                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect(host, Convert.ToInt32(port));
                smtp.Authenticate(r);
                return smtp;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }
        private SmtpClient GetSmtpClient(IServiceProvider provider)
        {
            return provider.GetService<SmtpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email Api v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                var serviceProvider = app.ApplicationServices;

                var smtpClient = serviceProvider.GetService<SmtpClient>();
                smtpClient.Dispose();

                Console.Write("I'm on app shutdown event");
            });
        }
    }
}