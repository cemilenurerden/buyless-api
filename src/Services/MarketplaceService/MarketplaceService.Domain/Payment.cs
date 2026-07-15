namespace MarketplaceService.Domain;

// Şu an sadece placeholder - Iyzico entegrasyonu Aşama 2'de eklenecek.
// Tablo veritabanında oluşturulacak ama henüz gerçek ödeme akışı yok.
public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid PayerId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? IyzicoPaymentId { get; private set; }   // Iyzico'dan dönen ID (Aşama 2)
    public string? IyzicoToken { get; private set; }        // Iyzico token (Aşama 2)
    public PaymentMethod Method { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Order Order { get; private set; } = null!;

    private Payment() { }

    public Payment(Guid orderId, Guid payerId, decimal amount, PaymentMethod method)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        PayerId = payerId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // Aşama 2'de Iyzico webhook'u geldiğinde çağrılacak
    public void MarkAsSuccessful(string iyzicoPaymentId, string iyzicoToken)
    {
        IyzicoPaymentId = iyzicoPaymentId;
        IyzicoToken = iyzicoToken;
        Status = PaymentStatus.Success;
        PaidAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
    }
}