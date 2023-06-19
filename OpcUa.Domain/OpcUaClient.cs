using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain.Contracts.Client;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;

namespace OpcUa.Domain;


public class OpcUaClient : IOpcClient
{
	public ObservableCollection<OpcNode> SubscribedNodes { get; private set; } = new();
	public ReplaySubject<bool> IsOnline { get; set; } = new(1);

	public ApplicationConfiguration SessionApplicationConfig
	{
		get => sessionApplicationConfig;
		private set
		{
			sessionApplicationConfig = value;
			sessionApplicationConfig.CertificateValidator.CertificateValidation += CertificateValidationNotification;
		}
	}
	private ApplicationConfiguration sessionApplicationConfig;
	/// <summary>
	/// Provides the session being established with an OPC UA server.
	/// </summary>
	public Session Session
	{
		get => mSession;
		set
		{
			mSession = value;
			mSession.KeepAlive += KeepAliveNotification;
			mSession.Notification += NotificationEventNotification;
		}
	}
	private Session mSession;

	public OpcUaClient(ApplicationConfiguration SessionApplicationConfig)
	{
		this.SessionApplicationConfig = SessionApplicationConfig;
		KeepAliveNotification += Notification_KeepAlive;
		CertificateValidationNotification += Notification_ServerCertificate;
	}


	/// <summary>
	/// Provides the event handling for server certificates.
	/// </summary>
	public CertificateValidationEventHandler CertificateValidationNotification { get; set; }

	/// <summary>
	/// Provides the event for a monitored event item.
	/// </summary>
	public NotificationEventHandler NotificationEventNotification { get; set; }

	/// <summary>
	/// Provides the event for KeepAliveNotifications.
	/// </summary>
	public KeepAliveEventHandler KeepAliveNotification { get; set; }


	private async void Notification_KeepAlive(ISession sender, KeepAliveEventArgs e)
	{
		try
		{
			if (!ServiceResult.IsGood(e.Status))
			{
				IsOnline.OnNext(false);
				mSession.Reconnect();
			}
			else
			{
				IsOnline.OnNext(true);
			}
			//if (!ReferenceEquals(sender, mSession)) return;
		}
		catch (Exception ex)
		{

		}
	}
	private async void Notification_ServerCertificate(CertificateValidator cert, CertificateValidationEventArgs e)
	{
		X509CertificateCollection certCol;
		using (X509Store store = new(StoreName.Root, StoreLocation.CurrentUser))
		{
			store.Open(OpenFlags.ReadOnly);
			certCol = store.Certificates.Find(X509FindType.FindByThumbprint, e.Certificate.Thumbprint, true);
		}
		if (certCol.Capacity > 0) e.Accept = true;
	}
}
