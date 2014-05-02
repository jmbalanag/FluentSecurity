﻿using System.Linq;
using System.Web.Mvc;
using FluentSecurity.Configuration;
using FluentSecurity.Core;
using FluentSecurity.Diagnostics;
using FluentSecurity.Policy.ViolationHandlers;
using FluentSecurity.Policy.ViolationHandlers.Conventions;

namespace FluentSecurity.ServiceLocation
{
	public class MvcRegistry : IRegistry
	{
		public void Configure(IContainer container)
		{
			container.Register<ISecurityConfiguration>(ctx => SecurityConfiguration.Get<MvcConfiguration>());
			container.Register<ISecurityHandler<ActionResult>>(ctx => new SecurityHandler(), Lifecycle.Singleton);

			container.Register<ISecurityContext>(ctx => ctx.Resolve<ISecurityConfiguration>().CreateContext());

			var controllerNameResolver = new MvcControllerNameResolver();
			var actionNameResolver = new MvcActionNameResolver();
			var actionResolver = new MvcActionResolver(actionNameResolver);

			container.Register<IControllerNameResolver<AuthorizationContext>>(ctx => controllerNameResolver, Lifecycle.Singleton);
			container.Register<IControllerNameResolver>(ctx => controllerNameResolver, Lifecycle.Singleton);
			container.Register<IActionNameResolver<AuthorizationContext>>(ctx => actionNameResolver, Lifecycle.Singleton);
			container.Register<IActionNameResolver>(ctx => actionNameResolver, Lifecycle.Singleton);
			container.Register<IActionResolver>(ctx => actionResolver, Lifecycle.Singleton);

			container.Register<IPolicyViolationHandler>(ctx => new DelegatePolicyViolationHandler(ctx.ResolveAll<IPolicyViolationHandler>()), Lifecycle.Singleton);

			container.Register<IPolicyViolationHandlerSelector<ActionResult>>(ctx => new PolicyViolationHandlerSelector(
				ctx.Resolve<ISecurityConfiguration>().Runtime.Conventions.OfType<IPolicyViolationHandlerConvention>()
				));

			container.Register<IWhatDoIHaveBuilder>(ctx => new DefaultWhatDoIHaveBuilder(), Lifecycle.Singleton);
		}
	}
}