//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
#pragma warning disable IDE0058 // Expression value is never used
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class QuiltContext : DbContext
    {
        public QuiltContext()
        {
        }

        public QuiltContext(DbContextOptions<QuiltContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountingYear> AccountingYears { get; set; }
        public virtual DbSet<AccountingYearStatusType> AccountingYearStatusTypes { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<AlertEmailRequest> AlertEmailRequests { get; set; }
        public virtual DbSet<AlertType> AlertTypes { get; set; }
        public virtual DbSet<Artifact> Artifacts { get; set; }
        public virtual DbSet<ArtifactType> ArtifactTypes { get; set; }
        public virtual DbSet<ArtifactValueType> ArtifactValueTypes { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Design> Designs { get; set; }
        public virtual DbSet<DesignBookmark> DesignBookmarks { get; set; }
        public virtual DbSet<DesignSnapshot> DesignSnapshots { get; set; }
        public virtual DbSet<EmailRequest> EmailRequests { get; set; }
        public virtual DbSet<Fulfillable> Fulfillables { get; set; }
        public virtual DbSet<FulfillableAddress> FulfillableAddresses { get; set; }
        public virtual DbSet<FulfillableEvent> FulfillableEvents { get; set; }
        public virtual DbSet<FulfillableItem> FulfillableItems { get; set; }
        public virtual DbSet<FulfillableItemComponent> FulfillableItemComponents { get; set; }
        public virtual DbSet<FulfillableSummaryView> FulfillableSummaryViews { get; set; }
        public virtual DbSet<FulfillableToReturnRequestView> FulfillableToReturnRequestViews { get; set; }
        public virtual DbSet<FulfillableToReturnView> FulfillableToReturnViews { get; set; }
        public virtual DbSet<FulfillableToShipmentRequestView> FulfillableToShipmentRequestViews { get; set; }
        public virtual DbSet<FulfillableToShipmentView> FulfillableToShipmentViews { get; set; }
        public virtual DbSet<FulfillableTransaction> FulfillableTransactions { get; set; }
        public virtual DbSet<FulfillableTransactionItem> FulfillableTransactionItems { get; set; }
        public virtual DbSet<Fundable> Fundables { get; set; }
        public virtual DbSet<FundableEvent> FundableEvents { get; set; }
        public virtual DbSet<FundableTransaction> FundableTransactions { get; set; }
        public virtual DbSet<Funder> Funders { get; set; }
        public virtual DbSet<FunderAccount> FunderAccounts { get; set; }
        public virtual DbSet<FunderEvent> FunderEvents { get; set; }
        public virtual DbSet<FunderTransaction> FunderTransactions { get; set; }
        public virtual DbSet<IncomeStatementReportInstance> IncomeStatementReportInstances { get; set; }
        public virtual DbSet<InventoryItem> InventoryItems { get; set; }
        public virtual DbSet<InventoryItemStock> InventoryItemStocks { get; set; }
        public virtual DbSet<InventoryItemStockTransaction> InventoryItemStockTransactions { get; set; }
        public virtual DbSet<InventoryItemStockTransactionItem> InventoryItemStockTransactionItems { get; set; }
        public virtual DbSet<InventoryItemTag> InventoryItemTags { get; set; }
        public virtual DbSet<InventoryItemTransaction> InventoryItemTransactions { get; set; }
        public virtual DbSet<InventoryItemTransactionType> InventoryItemTransactionTypes { get; set; }
        public virtual DbSet<InventoryItemType> InventoryItemTypes { get; set; }
        public virtual DbSet<InventoryItemUnit> InventoryItemUnits { get; set; }
        public virtual DbSet<KansasTaxTable> KansasTaxTables { get; set; }
        public virtual DbSet<KansasTaxTableEntry> KansasTaxTableEntries { get; set; }
        public virtual DbSet<LedgerAccount> LedgerAccounts { get; set; }
        public virtual DbSet<LedgerAccountSubtotal> LedgerAccountSubtotals { get; set; }
        public virtual DbSet<LedgerTransaction> LedgerTransactions { get; set; }
        public virtual DbSet<LedgerTransactionEntry> LedgerTransactionEntries { get; set; }
        public virtual DbSet<LogEntry> LogEntries { get; set; }
        public virtual DbSet<LogEntryType> LogEntryTypes { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageEmailRequest> MessageEmailRequests { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationEmailRequest> NotificationEmailRequests { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderBillingAddress> OrderBillingAddresses { get; set; }
        public virtual DbSet<OrderEvent> OrderEvents { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderNumber> OrderNumbers { get; set; }
        public virtual DbSet<OrderShippingAddress> OrderShippingAddresses { get; set; }
        public virtual DbSet<OrderStatusType> OrderStatusTypes { get; set; }
        public virtual DbSet<OrderTransaction> OrderTransactions { get; set; }
        public virtual DbSet<OrderTransactionItem> OrderTransactionItems { get; set; }
        public virtual DbSet<OrderTransactionType> OrderTransactionTypes { get; set; }
        public virtual DbSet<Orderable> Orderables { get; set; }
        public virtual DbSet<OrderableComponent> OrderableComponents { get; set; }
        public virtual DbSet<Orderer> Orderers { get; set; }
        public virtual DbSet<OrdererPendingOrder> OrdererPendingOrders { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<PricingSchedule> PricingSchedules { get; set; }
        public virtual DbSet<PricingScheduleEntry> PricingScheduleEntries { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectSnapshot> ProjectSnapshots { get; set; }
        public virtual DbSet<ProjectSnapshotComponent> ProjectSnapshotComponents { get; set; }
        public virtual DbSet<ProjectType> ProjectTypes { get; set; }
        public virtual DbSet<ReportInstance> ReportInstances { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<ResourceLibrary> ResourceLibraries { get; set; }
        public virtual DbSet<ResourceTag> ResourceTags { get; set; }
        public virtual DbSet<ResourceType> ResourceTypes { get; set; }
        public virtual DbSet<Return> Returns { get; set; }
        public virtual DbSet<ReturnEvent> ReturnEvents { get; set; }
        public virtual DbSet<ReturnItem> ReturnItems { get; set; }
        public virtual DbSet<ReturnRequest> ReturnRequests { get; set; }
        public virtual DbSet<ReturnRequestEvent> ReturnRequestEvents { get; set; }
        public virtual DbSet<ReturnRequestItem> ReturnRequestItems { get; set; }
        public virtual DbSet<ReturnRequestReason> ReturnRequestReasons { get; set; }
        public virtual DbSet<ReturnRequestStatusType> ReturnRequestStatusTypes { get; set; }
        public virtual DbSet<ReturnRequestSummaryView> ReturnRequestSummaryViews { get; set; }
        public virtual DbSet<ReturnRequestToReturnView> ReturnRequestToReturnViews { get; set; }
        public virtual DbSet<ReturnRequestTransaction> ReturnRequestTransactions { get; set; }
        public virtual DbSet<ReturnRequestType> ReturnRequestTypes { get; set; }
        public virtual DbSet<ReturnStatusType> ReturnStatusTypes { get; set; }
        public virtual DbSet<ReturnSummaryView> ReturnSummaryViews { get; set; }
        public virtual DbSet<ReturnTransaction> ReturnTransactions { get; set; }
        public virtual DbSet<Shipment> Shipments { get; set; }
        public virtual DbSet<ShipmentAddress> ShipmentAddresses { get; set; }
        public virtual DbSet<ShipmentEvent> ShipmentEvents { get; set; }
        public virtual DbSet<ShipmentItem> ShipmentItems { get; set; }
        public virtual DbSet<ShipmentRequest> ShipmentRequests { get; set; }
        public virtual DbSet<ShipmentRequestAddress> ShipmentRequestAddresses { get; set; }
        public virtual DbSet<ShipmentRequestEvent> ShipmentRequestEvents { get; set; }
        public virtual DbSet<ShipmentRequestItem> ShipmentRequestItems { get; set; }
        public virtual DbSet<ShipmentRequestStatusType> ShipmentRequestStatusTypes { get; set; }
        public virtual DbSet<ShipmentRequestSummaryView> ShipmentRequestSummaryViews { get; set; }
        public virtual DbSet<ShipmentRequestToShipmentView> ShipmentRequestToShipmentViews { get; set; }
        public virtual DbSet<ShipmentRequestTransaction> ShipmentRequestTransactions { get; set; }
        public virtual DbSet<ShipmentStatusType> ShipmentStatusTypes { get; set; }
        public virtual DbSet<ShipmentSummaryView> ShipmentSummaryViews { get; set; }
        public virtual DbSet<ShipmentTransaction> ShipmentTransactions { get; set; }
        public virtual DbSet<ShippingVendor> ShippingVendors { get; set; }
        public virtual DbSet<SquareCustomer> SquareCustomers { get; set; }
        public virtual DbSet<SquarePayload> SquarePayloads { get; set; }
        public virtual DbSet<SquarePayment> SquarePayments { get; set; }
        public virtual DbSet<SquarePaymentEvent> SquarePaymentEvents { get; set; }
        public virtual DbSet<SquarePaymentPayload> SquarePaymentPayloads { get; set; }
        public virtual DbSet<SquarePaymentTransaction> SquarePaymentTransactions { get; set; }
        public virtual DbSet<SquareRefund> SquareRefunds { get; set; }
        public virtual DbSet<SquareRefundEvent> SquareRefundEvents { get; set; }
        public virtual DbSet<SquareRefundPayload> SquareRefundPayloads { get; set; }
        public virtual DbSet<SquareRefundTransaction> SquareRefundTransactions { get; set; }
        public virtual DbSet<SquareWebPaymentRequest> SquareWebPaymentRequests { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TagCategory> TagCategories { get; set; }
        public virtual DbSet<TagType> TagTypes { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<TopicField> TopicFields { get; set; }
        public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<UserProfileAspNetUser> UserProfileAspNetUsers { get; set; }
        public virtual DbSet<UserProperty> UserProperties { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<WebsiteProperty> WebsiteProperties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountingYear>(entity =>
            {
                entity.HasKey(e => e.Year);

                entity.ToTable("AccountingYear");

                entity.HasIndex(e => e.AccountingYearStatusCode);

                entity.Property(e => e.Year).ValueGeneratedNever();

                entity.Property(e => e.AccountingYearStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.AccountingYearStatusCodeNavigation)
                    .WithMany(p => p.AccountingYears)
                    .HasForeignKey(d => d.AccountingYearStatusCode);
            });

            modelBuilder.Entity<AccountingYearStatusType>(entity =>
            {
                entity.HasKey(e => e.AccountingYearStatusCode);

                entity.ToTable("AccountingYearStatusType");

                entity.Property(e => e.AccountingYearStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AddressType>(entity =>
            {
                entity.HasKey(e => e.AddressTypeCode);

                entity.ToTable("AddressType");

                entity.Property(e => e.AddressTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Alert>(entity =>
            {
                entity.ToTable("Alert");

                entity.HasIndex(e => e.AlertTypeCode);

                entity.HasIndex(e => e.EmailRequestId);

                entity.HasIndex(e => e.ParticipantId);

                entity.HasIndex(e => e.TopicId);

                entity.Property(e => e.AlertId).HasDefaultValueSql("(NEXT VALUE FOR [AlertSequence])");

                entity.Property(e => e.AcknowledgementDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.AlertTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Exception).IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.AlertTypeCodeNavigation)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(d => d.AlertTypeCode);

                entity.HasOne(d => d.EmailRequest)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(d => d.EmailRequestId);

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(d => d.ParticipantId);

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(d => d.TopicId);
            });

            modelBuilder.Entity<AlertEmailRequest>(entity =>
            {
                entity.ToTable("AlertEmailRequest");

                entity.HasIndex(e => e.AlertId);

                entity.HasIndex(e => e.EmailRequestId);

                entity.Property(e => e.AlertEmailRequestId).HasDefaultValueSql("(NEXT VALUE FOR [AlertEmailRequestSequence])");

                entity.HasOne(d => d.Alert)
                    .WithMany(p => p.AlertEmailRequests)
                    .HasForeignKey(d => d.AlertId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.EmailRequest)
                    .WithMany(p => p.AlertEmailRequests)
                    .HasForeignKey(d => d.EmailRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AlertType>(entity =>
            {
                entity.HasKey(e => e.AlertTypeCode);

                entity.ToTable("AlertType");

                entity.Property(e => e.AlertTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Artifact>(entity =>
            {
                entity.ToTable("Artifact");

                entity.HasIndex(e => e.ArtifactTypeCode);

                entity.HasIndex(e => e.ArtifactValueTypeCode);

                entity.Property(e => e.ArtifactId).HasDefaultValueSql("(NEXT VALUE FOR [ArtifactSequence])");

                entity.Property(e => e.ArtifactTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ArtifactValueTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.ArtifactTypeCodeNavigation)
                    .WithMany(p => p.Artifacts)
                    .HasForeignKey(d => d.ArtifactTypeCode);

                entity.HasOne(d => d.ArtifactValueTypeCodeNavigation)
                    .WithMany(p => p.Artifacts)
                    .HasForeignKey(d => d.ArtifactValueTypeCode);
            });

            modelBuilder.Entity<ArtifactType>(entity =>
            {
                entity.HasKey(e => e.ArtifactTypeCode);

                entity.ToTable("ArtifactType");

                entity.Property(e => e.ArtifactTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ArtifactValueType>(entity =>
            {
                entity.HasKey(e => e.ArtifactValueTypeCode);

                entity.ToTable("ArtifactValueType");

                entity.Property(e => e.ArtifactValueTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CountryCode);

                entity.ToTable("Country");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Design>(entity =>
            {
                entity.ToTable("Design");

                entity.HasIndex(e => e.OwnerId);

                entity.Property(e => e.DesignId).ValueGeneratedNever();

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.DeleteDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Designs)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DesignBookmark>(entity =>
            {
                entity.ToTable("DesignBookmark");

                entity.Property(e => e.DesignBookmarkId).ValueGeneratedNever();
            });

            modelBuilder.Entity<DesignSnapshot>(entity =>
            {
                entity.ToTable("DesignSnapshot");

                entity.HasIndex(e => e.ArtifactId);

                entity.HasIndex(e => e.DesignId);

                entity.Property(e => e.DesignSnapshotId).HasDefaultValueSql("(NEXT VALUE FOR [DesignSnapshotSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Artifact)
                    .WithMany(p => p.DesignSnapshots)
                    .HasForeignKey(d => d.ArtifactId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Design)
                    .WithMany(p => p.DesignSnapshots)
                    .HasForeignKey(d => d.DesignId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EmailRequest>(entity =>
            {
                entity.ToTable("EmailRequest");

                entity.HasIndex(e => e.EmailRequestStatusCode);

                entity.HasIndex(e => e.RecipientParticipantId);

                entity.Property(e => e.EmailRequestId).HasDefaultValueSql("(NEXT VALUE FOR [EmailRequestSequence])");

                entity.Property(e => e.BodyText).IsRequired();

                entity.Property(e => e.BodyTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EmailRequestStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailRequestStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RecipientEmail)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.RecipientEmailName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SenderEmail)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.SenderEmailName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.RecipientParticipant)
                    .WithMany(p => p.EmailRequests)
                    .HasForeignKey(d => d.RecipientParticipantId);
            });

            modelBuilder.Entity<Fulfillable>(entity =>
            {
                entity.ToTable("Fulfillable");

                entity.HasIndex(e => e.FulfillableReference)
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.FulfillableId).HasDefaultValueSql("(NEXT VALUE FOR [FulfillableSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FulfillableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FulfillableStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FulfillableStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<FulfillableAddress>(entity =>
            {
                entity.HasKey(e => e.FulfillableId);

                entity.ToTable("FulfillableAddress");

                entity.Property(e => e.FulfillableId).ValueGeneratedNever();

                entity.Property(e => e.ShipToAddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToAddressLine2).HasMaxLength(100);

                entity.Property(e => e.ShipToCity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShipToCountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.ShipToName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToPostalCode)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.ShipToStateCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Fulfillable)
                    .WithOne(p => p.FulfillableAddress)
                    .HasForeignKey<FulfillableAddress>(d => d.FulfillableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FulfillableEvent>(entity =>
            {
                entity.ToTable("FulfillableEvent");

                entity.HasIndex(e => e.FulfillableTransactionId);

                entity.Property(e => e.FulfillableEventId).HasDefaultValueSql("(NEXT VALUE FOR [FulfillableEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.FulfillableTransaction)
                    .WithMany(p => p.FulfillableEvents)
                    .HasForeignKey(d => d.FulfillableTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FulfillableItem>(entity =>
            {
                entity.ToTable("FulfillableItem");

                entity.HasComment("A single item to be fulfilled.");

                entity.HasIndex(e => e.FulfillableId);

                entity.HasIndex(e => e.FulfillableItemReference)
                    .IsUnique();

                entity.Property(e => e.FulfillableItemId).HasDefaultValueSql("(NEXT VALUE FOR [FulfillableItemSequence])");

                entity.Property(e => e.CompleteQuantity).HasComment("The number of units that have been successfully fulfilled.  This value should exceed the request quantity.");

                entity.Property(e => e.ConsumableReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FulfillableItemReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RequestQuantity).HasComment("Number of units to be fuilfilled.  This value is specified when the entity is created and cannot be modified.");

                entity.Property(e => e.ReturnQuantity).HasComment("The number of units that have been returned.  This value should not exceed the complete quantity.");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Fulfillable)
                    .WithMany(p => p.FulfillableItems)
                    .HasForeignKey(d => d.FulfillableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FulfillableItemComponent>(entity =>
            {
                entity.ToTable("FulfillableItemComponent");

                entity.HasIndex(e => e.FulfillableItemId);

                entity.Property(e => e.FulfillableItemComponentId).HasDefaultValueSql("(NEXT VALUE FOR [FulfillableItemComponentSequence])");

                entity.Property(e => e.ConsumableReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.FulfillableItem)
                    .WithMany(p => p.FulfillableItemComponents)
                    .HasForeignKey(d => d.FulfillableItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FulfillableSummaryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FulfillableSummaryView");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FulfillableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FulfillableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FulfillableStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FulfillableStatusDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<FulfillableToReturnRequestView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FulfillableToReturnRequestView");
            });

            modelBuilder.Entity<FulfillableToReturnView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FulfillableToReturnView");
            });

            modelBuilder.Entity<FulfillableToShipmentRequestView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FulfillableToShipmentRequestView");
            });

            modelBuilder.Entity<FulfillableToShipmentView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FulfillableToShipmentView");
            });

            modelBuilder.Entity<FulfillableTransaction>(entity =>
            {
                entity.ToTable("FulfillableTransaction");

                entity.Property(e => e.FulfillableTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [FulfillableTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FulfillableTransactionItem>(entity =>
            {
                entity.ToTable("FulfillableTransactionItem");

                entity.HasIndex(e => e.FulfillableItemId);

                entity.HasIndex(e => e.FulfillableTransactionId);

                entity.Property(e => e.FulfillableTransactionItemId).HasDefaultValueSql("(NEXT VALUE FOR [FulfillableTransactionItemSequence])");

                entity.HasOne(d => d.FulfillableItem)
                    .WithMany(p => p.FulfillableTransactionItems)
                    .HasForeignKey(d => d.FulfillableItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.FulfillableTransaction)
                    .WithMany(p => p.FulfillableTransactionItems)
                    .HasForeignKey(d => d.FulfillableTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Fundable>(entity =>
            {
                entity.ToTable("Fundable");

                entity.HasIndex(e => e.FundableReference)
                    .IsUnique();

                entity.Property(e => e.FundableId).HasDefaultValueSql("(NEXT VALUE FOR [FundableSequence])");

                entity.Property(e => e.FundableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FundsReceived).HasColumnType("money");

                entity.Property(e => e.FundsRequiredIncome).HasColumnType("money");

                entity.Property(e => e.FundsRequiredSalesTax).HasColumnType("money");

                entity.Property(e => e.FundsRequiredSalesTaxJurisdiction)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FundsRequiredTotal).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<FundableEvent>(entity =>
            {
                entity.ToTable("FundableEvent");

                entity.HasIndex(e => e.FundableTransactionId);

                entity.Property(e => e.FundableEventId).HasDefaultValueSql("(NEXT VALUE FOR [FundableEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.FundableTransaction)
                    .WithMany(p => p.FundableEvents)
                    .HasForeignKey(d => d.FundableTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FundableTransaction>(entity =>
            {
                entity.ToTable("FundableTransaction");

                entity.HasIndex(e => e.FundableId);

                entity.Property(e => e.FundableTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [FundableTransactionsequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.FundsReceived).HasColumnType("money");

                entity.Property(e => e.FundsRequiredIncome).HasColumnType("money");

                entity.Property(e => e.FundsRequiredSalesTax).HasColumnType("money");

                entity.Property(e => e.FundsRequiredSalesTaxJurisdiction)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Fundable)
                    .WithMany(p => p.FundableTransactions)
                    .HasForeignKey(d => d.FundableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Funder>(entity =>
            {
                entity.ToTable("Funder");

                entity.HasIndex(e => e.FunderReference)
                    .IsUnique();

                entity.Property(e => e.FunderId).HasDefaultValueSql("(NEXT VALUE FOR [FunderSequence])");

                entity.Property(e => e.FunderReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<FunderAccount>(entity =>
            {
                entity.HasKey(e => new { e.FunderId, e.FundableReference });

                entity.ToTable("FunderAccount");

                entity.Property(e => e.FundableReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FundsAvailable).HasColumnType("money");

                entity.Property(e => e.FundsReceived).HasColumnType("money");

                entity.Property(e => e.FundsRefundable).HasColumnType("money");

                entity.Property(e => e.FundsRefunded).HasColumnType("money");

                entity.Property(e => e.ProcessingFee).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Funder)
                    .WithMany(p => p.FunderAccounts)
                    .HasForeignKey(d => d.FunderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FunderEvent>(entity =>
            {
                entity.ToTable("FunderEvent");

                entity.HasIndex(e => e.FunderTransactionId);

                entity.Property(e => e.FunderEventId).HasDefaultValueSql("(NEXT VALUE FOR [FunderEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.FunderTransaction)
                    .WithMany(p => p.FunderEvents)
                    .HasForeignKey(d => d.FunderTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FunderTransaction>(entity =>
            {
                entity.ToTable("FunderTransaction");

                entity.HasIndex(e => e.FunderId);

                entity.HasIndex(e => new { e.FunderId, e.FundableReference });

                entity.Property(e => e.FunderTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [FunderTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.FundableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FundsAvailable).HasColumnType("money");

                entity.Property(e => e.FundsReceived).HasColumnType("money");

                entity.Property(e => e.FundsRefundable).HasColumnType("money");

                entity.Property(e => e.FundsRefunded).HasColumnType("money");

                entity.Property(e => e.ProcessingFee).HasColumnType("money");

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Fund)
                    .WithMany(p => p.FunderTransactions)
                    .HasForeignKey(d => new { d.FunderId, d.FundableReference })
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<IncomeStatementReportInstance>(entity =>
            {
                entity.HasKey(e => new { e.ReportInstanceId, e.OrderLedgerAccountTypeId, e.OrderLedgerAccountTypeName, e.Amount });

                entity.ToTable("IncomeStatementReportInstance");

                entity.HasIndex(e => e.ReportInstanceId);

                entity.Property(e => e.OrderLedgerAccountTypeName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.HasOne(d => d.ReportInstance)
                    .WithMany(p => p.IncomeStatementReportInstances)
                    .HasForeignKey(d => d.ReportInstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.ToTable("InventoryItem");

                entity.HasIndex(e => e.InventoryItemTypeCode);

                entity.HasIndex(e => e.PricingScheduleId);

                entity.Property(e => e.InventoryItemId).HasDefaultValueSql("(NEXT VALUE FOR [InventoryItemSequence])");

                entity.Property(e => e.InventoryItemTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.InventoryItemTypeCodeNavigation)
                    .WithMany(p => p.InventoryItems)
                    .HasForeignKey(d => d.InventoryItemTypeCode);

                entity.HasOne(d => d.PricingSchedule)
                    .WithMany(p => p.InventoryItems)
                    .HasForeignKey(d => d.PricingScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<InventoryItemStock>(entity =>
            {
                entity.ToTable("InventoryItemStock");

                entity.HasIndex(e => e.InventoryItemId);

                entity.HasIndex(e => e.UnitOfMeasureCode);

                entity.Property(e => e.InventoryItemStockId).HasDefaultValueSql("(NEXT VALUE FOR [InventoryItemStockSequence])");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.StockDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitCost).HasColumnType("money");

                entity.Property(e => e.UnitOfMeasureCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.InventoryItem)
                    .WithMany(p => p.InventoryItemStocks)
                    .HasForeignKey(d => d.InventoryItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UnitOfMeasureCodeNavigation)
                    .WithMany(p => p.InventoryItemStocks)
                    .HasForeignKey(d => d.UnitOfMeasureCode);
            });

            modelBuilder.Entity<InventoryItemStockTransaction>(entity =>
            {
                entity.ToTable("InventoryItemStockTransaction");

                entity.HasIndex(e => e.LedgerAccountTransactionId);

                entity.HasIndex(e => e.OrderLedgerAccountTransactionId);

                entity.Property(e => e.InventoryItemStockTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [InventoryItemStockTransactionSequence])");

                entity.Property(e => e.TotalCost).HasColumnType("money");

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.LedgerAccountTransaction)
                    .WithMany(p => p.InventoryItemStockTransactions)
                    .HasForeignKey(d => d.LedgerAccountTransactionId);
            });

            modelBuilder.Entity<InventoryItemStockTransactionItem>(entity =>
            {
                entity.ToTable("InventoryItemStockTransactionItem");

                entity.HasIndex(e => e.InventoryItemStockId);

                entity.HasIndex(e => e.InventoryItemStockTransactionId);

                entity.Property(e => e.InventoryItemStockTransactionItemId).HasDefaultValueSql("(NEXT VALUE FOR [InventoryItemStockTransactionItemSequence])");

                entity.Property(e => e.Cost).HasColumnType("money");

                entity.HasOne(d => d.InventoryItemStock)
                    .WithMany(p => p.InventoryItemStockTransactionItems)
                    .HasForeignKey(d => d.InventoryItemStockId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.InventoryItemStockTransaction)
                    .WithMany(p => p.InventoryItemStockTransactionItems)
                    .HasForeignKey(d => d.InventoryItemStockTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<InventoryItemTag>(entity =>
            {
                entity.HasKey(e => new { e.InventoryItemId, e.TagId });

                entity.ToTable("InventoryItemTag");

                entity.HasIndex(e => e.InventoryItemId);

                entity.HasIndex(e => e.TagId);

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.InventoryItem)
                    .WithMany(p => p.InventoryItemTags)
                    .HasForeignKey(d => d.InventoryItemId);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.InventoryItemTags)
                    .HasForeignKey(d => d.TagId);
            });

            modelBuilder.Entity<InventoryItemTransaction>(entity =>
            {
                entity.ToTable("InventoryItemTransaction");

                entity.HasIndex(e => e.AspNetUserId);

                entity.HasIndex(e => e.InventoryItemId);

                entity.HasIndex(e => e.InventoryItemTransactionTypeCode);

                entity.HasIndex(e => e.VendorId);

                entity.Property(e => e.InventoryItemTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [InventoryItemTransactionSequence])");

                entity.Property(e => e.InventoryItemTransactionTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.AspNetUser)
                    .WithMany(p => p.InventoryItemTransactions)
                    .HasForeignKey(d => d.AspNetUserId);

                entity.HasOne(d => d.InventoryItem)
                    .WithMany(p => p.InventoryItemTransactions)
                    .HasForeignKey(d => d.InventoryItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.InventoryItemTransactionTypeCodeNavigation)
                    .WithMany(p => p.InventoryItemTransactions)
                    .HasForeignKey(d => d.InventoryItemTransactionTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.InventoryItemTransactions)
                    .HasForeignKey(d => d.VendorId);
            });

            modelBuilder.Entity<InventoryItemTransactionType>(entity =>
            {
                entity.HasKey(e => e.InventoryItemTransactionTypeCode);

                entity.ToTable("InventoryItemTransactionType");

                entity.Property(e => e.InventoryItemTransactionTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InventoryItemType>(entity =>
            {
                entity.HasKey(e => e.InventoryItemTypeCode);

                entity.ToTable("InventoryItemType");

                entity.Property(e => e.InventoryItemTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InventoryItemUnit>(entity =>
            {
                entity.HasKey(e => new { e.InventoryItemId, e.UnitOfMeasureCode });

                entity.ToTable("InventoryItemUnit");

                entity.HasIndex(e => e.InventoryItemId);

                entity.HasIndex(e => e.UnitOfMeasureCode);

                entity.Property(e => e.UnitOfMeasureCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.InventoryItem)
                    .WithMany(p => p.InventoryItemUnits)
                    .HasForeignKey(d => d.InventoryItemId);

                entity.HasOne(d => d.UnitOfMeasureCodeNavigation)
                    .WithMany(p => p.InventoryItemUnits)
                    .HasForeignKey(d => d.UnitOfMeasureCode);
            });

            modelBuilder.Entity<KansasTaxTable>(entity =>
            {
                entity.ToTable("KansasTaxTable");

                entity.Property(e => e.KansasTaxTableId).HasDefaultValueSql("(NEXT VALUE FOR [KansasTaxTableSequence])");

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.Property(e => e.ExpirationDate).HasColumnType("date");
            });

            modelBuilder.Entity<KansasTaxTableEntry>(entity =>
            {
                entity.ToTable("KansasTaxTableEntry");

                entity.HasIndex(e => e.KansasTaxTableId);

                entity.Property(e => e.KansasTaxTableEntryId).HasDefaultValueSql("(NEXT VALUE FOR [KansasTaxTableEntrySequence])");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.County)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.InsideCityJurisdictionCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.InsideCityTaxRate).HasColumnType("money");

                entity.Property(e => e.OutsideCityJurisdictionCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.OutsideCityTaxRate).HasColumnType("money");

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.HasOne(d => d.KansasTaxTable)
                    .WithMany(p => p.KansasTaxTableEntries)
                    .HasForeignKey(d => d.KansasTaxTableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LedgerAccount>(entity =>
            {
                entity.HasKey(e => e.LedgerAccountNumber);

                entity.ToTable("LedgerAccount");

                entity.Property(e => e.LedgerAccountNumber).ValueGeneratedNever();

                entity.Property(e => e.DebitCreditCode)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LedgerAccountSubtotal>(entity =>
            {
                entity.ToTable("LedgerAccountSubtotal");

                entity.HasIndex(e => e.AccountingYear);

                entity.HasIndex(e => e.LedgerAccountNumber);

                entity.Property(e => e.LedgerAccountSubtotalId).HasDefaultValueSql("(NEXT VALUE FOR [LedgerAccountSubtotalSequence])");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.AccountingYearNavigation)
                    .WithMany(p => p.LedgerAccountSubtotals)
                    .HasForeignKey(d => d.AccountingYear)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LedgerAccountNumberNavigation)
                    .WithMany(p => p.LedgerAccountSubtotals)
                    .HasForeignKey(d => d.LedgerAccountNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LedgerTransaction>(entity =>
            {
                entity.ToTable("LedgerTransaction");

                entity.Property(e => e.LedgerTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [LedgerTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.PostDateTime).HasColumnType("datetime");

                entity.Property(e => e.TransactionAmount).HasColumnType("money");

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LedgerTransactionEntry>(entity =>
            {
                entity.ToTable("LedgerTransactionEntry");

                entity.HasIndex(e => e.LedgerAccountNumber);

                entity.HasIndex(e => e.LedgerAccountSubtotalId);

                entity.HasIndex(e => e.LedgerTransactionId);

                entity.Property(e => e.LedgerTransactionEntryId).HasDefaultValueSql("(NEXT VALUE FOR [LedgerTransactionEntrySequence])");

                entity.Property(e => e.DebitCreditCode)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.LedgerReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SalesTaxJurisdiction)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionEntryAmount).HasColumnType("money");

                entity.HasOne(d => d.LedgerAccountNumberNavigation)
                    .WithMany(p => p.LedgerTransactionEntries)
                    .HasForeignKey(d => d.LedgerAccountNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LedgerAccountSubtotal)
                    .WithMany(p => p.LedgerTransactionEntries)
                    .HasForeignKey(d => d.LedgerAccountSubtotalId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LedgerTransaction)
                    .WithMany(p => p.LedgerTransactionEntries)
                    .HasForeignKey(d => d.LedgerTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("LogEntry");

                entity.HasIndex(e => e.LogEntryTypeCode);

                entity.Property(e => e.LogEntryId).HasDefaultValueSql("(NEXT VALUE FOR [LogEntrySequence])");

                entity.Property(e => e.LogEntryTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LogName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SeverityCode)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.LogEntryTypeCodeNavigation)
                    .WithMany(p => p.LogEntries)
                    .HasForeignKey(d => d.LogEntryTypeCode);
            });

            modelBuilder.Entity<LogEntryType>(entity =>
            {
                entity.HasKey(e => e.LogEntryTypeCode);

                entity.ToTable("LogEntryType");

                entity.Property(e => e.LogEntryTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.HasIndex(e => e.ParticipantId);

                entity.HasIndex(e => e.TopicId);

                entity.Property(e => e.MessageId).HasDefaultValueSql("(NEXT VALUE FOR [MessageSequence])");

                entity.Property(e => e.AcknowledgementDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SendReceiveCode)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.TopicId);
            });

            modelBuilder.Entity<MessageEmailRequest>(entity =>
            {
                entity.ToTable("MessageEmailRequest");

                entity.HasIndex(e => e.EmailRequestId);

                entity.HasIndex(e => e.MessageId);

                entity.Property(e => e.MessageEmailRequestId).HasDefaultValueSql("(NEXT VALUE FOR [MessageEmailRequestSequence])");

                entity.HasOne(d => d.EmailRequest)
                    .WithMany(p => p.MessageEmailRequests)
                    .HasForeignKey(d => d.EmailRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.MessageEmailRequests)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.HasIndex(e => e.NotificationTypeCode);

                entity.HasIndex(e => e.ParticipantId);

                entity.HasIndex(e => e.TopicId);

                entity.Property(e => e.NotificationId).HasDefaultValueSql("(NEXT VALUE FOR [NotificationSequence])");

                entity.Property(e => e.AcknowledgementDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.NotificationTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.NotificationTypeCodeNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.NotificationTypeCode);

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.TopicId);
            });

            modelBuilder.Entity<NotificationEmailRequest>(entity =>
            {
                entity.ToTable("NotificationEmailRequest");

                entity.HasIndex(e => e.EmailRequestId);

                entity.HasIndex(e => e.NotificationId);

                entity.Property(e => e.NotificationEmailRequestId).HasDefaultValueSql("(NEXT VALUE FOR [NotificationEmailRequestSequence])");

                entity.HasOne(d => d.EmailRequest)
                    .WithMany(p => p.NotificationEmailRequests)
                    .HasForeignKey(d => d.EmailRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.NotificationEmailRequests)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.HasKey(e => e.NotificationTypeCode);

                entity.ToTable("NotificationType");

                entity.Property(e => e.NotificationTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Body).IsRequired();

                entity.Property(e => e.BodyTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasIndex(e => e.OrderStatusCode);

                entity.HasIndex(e => e.OrdererId);

                entity.Property(e => e.OrderId).HasDefaultValueSql("(NEXT VALUE FOR [OrderSequence])");

                entity.Property(e => e.Discount).HasColumnType("money");

                entity.Property(e => e.FulfillmentDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FundsReceived).HasColumnType("money");

                entity.Property(e => e.FundsRequired).HasColumnType("money");

                entity.Property(e => e.ItemSubtotal).HasColumnType("money");

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.OrderStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrderStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.PretaxAmount).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SalesTax).HasColumnType("money");

                entity.Property(e => e.SalesTaxRate).HasColumnType("decimal(6, 5)");

                entity.Property(e => e.SalexTaxJurisdiction)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Shipping).HasColumnType("money");

                entity.Property(e => e.SubmissionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.TaxableAmount).HasColumnType("money");

                entity.Property(e => e.TotalAmount).HasColumnType("money");

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.OrderStatusCodeNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Orderer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrdererId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrderBillingAddress>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.ToTable("OrderBillingAddress");

                entity.HasIndex(e => new { e.BillToStateCode, e.BillToCountryCode });

                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.BillToAddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BillToAddressLine2).HasMaxLength(100);

                entity.Property(e => e.BillToCity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BillToCountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.BillToName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BillToPostalCode)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.BillToStateCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.OrderBillingAddress)
                    .HasForeignKey<OrderBillingAddress>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.BillTo)
                    .WithMany(p => p.OrderBillingAddresses)
                    .HasForeignKey(d => new { d.BillToStateCode, d.BillToCountryCode })
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrderEvent>(entity =>
            {
                entity.ToTable("OrderEvent");

                entity.HasIndex(e => e.OrderTransactionId);

                entity.Property(e => e.OrderEventId).HasDefaultValueSql("(NEXT VALUE FOR [OrderEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.OrderTransaction)
                    .WithMany(p => p.OrderEvents)
                    .HasForeignKey(d => d.OrderTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItem");

                entity.HasIndex(e => e.OrderId);

                entity.HasIndex(e => e.OrderableId);

                entity.Property(e => e.OrderItemId).HasDefaultValueSql("(NEXT VALUE FOR [OrderItemSequence])");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.TotalPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Orderable)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrderNumber>(entity =>
            {
                entity.HasKey(e => e.OrderDateUtc);

                entity.ToTable("OrderNumber");

                entity.Property(e => e.OrderDateUtc).HasColumnType("date");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<OrderShippingAddress>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.ToTable("OrderShippingAddress");

                entity.HasIndex(e => new { e.ShipToStateCode, e.ShipToCountryCode });

                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.ShipToAddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToAddressLine2).HasMaxLength(100);

                entity.Property(e => e.ShipToCity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShipToCountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ShipToName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToPostalCode)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.ShipToStateCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.OrderShippingAddress)
                    .HasForeignKey<OrderShippingAddress>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ShipTo)
                    .WithMany(p => p.OrderShippingAddresses)
                    .HasForeignKey(d => new { d.ShipToStateCode, d.ShipToCountryCode })
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrderStatusType>(entity =>
            {
                entity.HasKey(e => e.OrderStatusCode);

                entity.ToTable("OrderStatusType");

                entity.Property(e => e.OrderStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrderTransaction>(entity =>
            {
                entity.ToTable("OrderTransaction");

                entity.HasIndex(e => e.OrderId);

                entity.HasIndex(e => e.OrderStatusCode);

                entity.HasIndex(e => e.OrderTransactionTypeCode);

                entity.Property(e => e.OrderTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [OrderTransactionSequence])");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Discount).HasColumnType("money");

                entity.Property(e => e.FundsReceived).HasColumnType("money");

                entity.Property(e => e.FundsRequired).HasColumnType("money");

                entity.Property(e => e.ItemSubtotal).HasColumnType("money");

                entity.Property(e => e.OrderStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrderTransactionTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SalesTax).HasColumnType("money");

                entity.Property(e => e.SalesTaxJurisdiction)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SalesTaxRate).HasColumnType("decimal(6, 5)");

                entity.Property(e => e.Shipping).HasColumnType("money");

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderTransactions)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OrderStatusCodeNavigation)
                    .WithMany(p => p.OrderTransactions)
                    .HasForeignKey(d => d.OrderStatusCode);

                entity.HasOne(d => d.OrderTransactionTypeCodeNavigation)
                    .WithMany(p => p.OrderTransactions)
                    .HasForeignKey(d => d.OrderTransactionTypeCode);
            });

            modelBuilder.Entity<OrderTransactionItem>(entity =>
            {
                entity.ToTable("OrderTransactionItem");

                entity.HasIndex(e => e.OrderItemId);

                entity.HasIndex(e => e.OrderTransactionId);

                entity.HasIndex(e => new { e.OrderTransactionId, e.OrderItemId });

                entity.Property(e => e.OrderTransactionItemId).HasDefaultValueSql("(NEXT VALUE FOR [OrderTransactionItemSequence])");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.OrderItem)
                    .WithMany(p => p.OrderTransactionItems)
                    .HasForeignKey(d => d.OrderItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.OrderTransaction)
                    .WithMany(p => p.OrderTransactionItems)
                    .HasForeignKey(d => d.OrderTransactionId);
            });

            modelBuilder.Entity<OrderTransactionType>(entity =>
            {
                entity.HasKey(e => e.OrderTransactionTypeCode);

                entity.ToTable("OrderTransactionType");

                entity.Property(e => e.OrderTransactionTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Orderable>(entity =>
            {
                entity.ToTable("Orderable");

                entity.HasIndex(e => e.OrderableReference)
                    .IsUnique();

                entity.Property(e => e.OrderableId).HasDefaultValueSql("(NEXT VALUE FOR [OrderableSequence])");

                entity.Property(e => e.ConsumableReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.OrderableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<OrderableComponent>(entity =>
            {
                entity.ToTable("OrderableComponent");

                entity.HasIndex(e => e.OrderableComponentReference)
                    .IsUnique();

                entity.HasIndex(e => e.OrderableId);

                entity.Property(e => e.OrderableComponentId).HasDefaultValueSql("(NEXT VALUE FOR [OrderableComponentSequence])");

                entity.Property(e => e.ConsumableReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.OrderableComponentReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.TotalPrice).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Orderable)
                    .WithMany(p => p.OrderableComponents)
                    .HasForeignKey(d => d.OrderableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Orderer>(entity =>
            {
                entity.ToTable("Orderer");

                entity.HasIndex(e => e.OrdererReference)
                    .IsUnique();

                entity.Property(e => e.OrdererId).HasDefaultValueSql("(NEXT VALUE FOR [OrdererSequence])");

                entity.Property(e => e.OrdererReference).IsRequired();

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<OrdererPendingOrder>(entity =>
            {
                entity.HasKey(e => e.OrdererId);

                entity.ToTable("OrdererPendingOrder");

                entity.HasIndex(e => e.OrderId);

                entity.Property(e => e.OrdererId).ValueGeneratedNever();

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrdererPendingOrders)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Orderer)
                    .WithOne(p => p.OrdererPendingOrder)
                    .HasForeignKey<OrdererPendingOrder>(d => d.OrdererId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("Owner");

                entity.HasIndex(e => e.OwnerReference)
                    .IsUnique();

                entity.Property(e => e.OwnerId).HasDefaultValueSql("(NEXT VALUE FOR [OwnerSequence])");

                entity.Property(e => e.OwnerReference).IsRequired();

                entity.Property(e => e.OwnerTypeCode)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("Participant");

                entity.HasIndex(e => e.ParticipantReference)
                    .IsUnique();

                entity.Property(e => e.ParticipantId).HasDefaultValueSql("(NEXT VALUE FOR [ParticipantSequence])");

                entity.Property(e => e.ParticipantReference).IsRequired();
            });

            modelBuilder.Entity<PricingSchedule>(entity =>
            {
                entity.ToTable("PricingSchedule");

                entity.Property(e => e.PricingScheduleId).HasDefaultValueSql("(NEXT VALUE FOR [PricingScheduleSequence])");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<PricingScheduleEntry>(entity =>
            {
                entity.ToTable("PricingScheduleEntry");

                entity.HasIndex(e => e.PricingScheduleId);

                entity.HasIndex(e => e.UnitOfMeasureCode);

                entity.Property(e => e.PricingScheduleEntryId).HasDefaultValueSql("(NEXT VALUE FOR [PricingScheduleEntrySequence])");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UnitOfMeasureCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PricingSchedule)
                    .WithMany(p => p.PricingScheduleEntries)
                    .HasForeignKey(d => d.PricingScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UnitOfMeasureCodeNavigation)
                    .WithMany(p => p.PricingScheduleEntries)
                    .HasForeignKey(d => d.UnitOfMeasureCode);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.HasIndex(e => e.OwnerId);

                entity.HasIndex(e => e.ProjectTypeCode);

                entity.Property(e => e.ProjectId).ValueGeneratedNever();

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.DeleteDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProjectTypeCodeNavigation)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ProjectTypeCode);
            });

            modelBuilder.Entity<ProjectSnapshot>(entity =>
            {
                entity.ToTable("ProjectSnapshot");

                entity.HasIndex(e => e.ArtifactId);

                entity.HasIndex(e => e.DesignSnapshotId);

                entity.HasIndex(e => e.ProjectId);

                entity.Property(e => e.ProjectSnapshotId).HasDefaultValueSql("(NEXT VALUE FOR [ProjectSnapshotSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Artifact)
                    .WithMany(p => p.ProjectSnapshots)
                    .HasForeignKey(d => d.ArtifactId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.DesignSnapshot)
                    .WithMany(p => p.ProjectSnapshots)
                    .HasForeignKey(d => d.DesignSnapshotId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectSnapshots)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ProjectSnapshotComponent>(entity =>
            {
                entity.ToTable("ProjectSnapshotComponent");

                entity.HasIndex(e => e.ProjectSnapshotId);

                entity.HasIndex(e => e.UnitOfMeasureCode);

                entity.Property(e => e.ProjectSnapshotComponentId).HasDefaultValueSql("(NEXT VALUE FOR [ProjectSnapshotComponentSequence])");

                entity.Property(e => e.ConsumableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitOfMeasureCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProjectSnapshot)
                    .WithMany(p => p.ProjectSnapshotComponents)
                    .HasForeignKey(d => d.ProjectSnapshotId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UnitOfMeasureCodeNavigation)
                    .WithMany(p => p.ProjectSnapshotComponents)
                    .HasForeignKey(d => d.UnitOfMeasureCode);
            });

            modelBuilder.Entity<ProjectType>(entity =>
            {
                entity.HasKey(e => e.ProjectTypeCode);

                entity.ToTable("ProjectType");

                entity.Property(e => e.ProjectTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportInstance>(entity =>
            {
                entity.ToTable("ReportInstance");

                entity.Property(e => e.ReportInstanceId).HasDefaultValueSql("(NEXT VALUE FOR [ReportInstanceSequence])");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.RunDate).HasColumnType("date");

                entity.Property(e => e.ThroughDate).HasColumnType("date");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("Resource");

                entity.HasIndex(e => e.ResourceLibraryId);

                entity.HasIndex(e => e.ResourceTypeId);

                entity.Property(e => e.ResourceId).HasDefaultValueSql("(NEXT VALUE FOR [ResourceSequence])");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ResourceData).IsRequired();

                entity.HasOne(d => d.ResourceLibrary)
                    .WithMany(p => p.Resources)
                    .HasForeignKey(d => d.ResourceLibraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ResourceType)
                    .WithMany(p => p.Resources)
                    .HasForeignKey(d => d.ResourceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ResourceLibrary>(entity =>
            {
                entity.ToTable("ResourceLibrary");

                entity.Property(e => e.ResourceLibraryId).HasDefaultValueSql("(NEXT VALUE FOR [ResourceLibrarySequence])");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ResourceTag>(entity =>
            {
                entity.HasKey(e => new { e.ResourceId, e.TagId });

                entity.ToTable("ResourceTag");

                entity.HasIndex(e => e.ResourceId);

                entity.HasIndex(e => e.TagId);

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.ResourceTags)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ResourceTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ResourceType>(entity =>
            {
                entity.ToTable("ResourceType");

                entity.Property(e => e.ResourceTypeId).HasDefaultValueSql("(NEXT VALUE FOR [ResourceTypeSequence])");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Return>(entity =>
            {
                entity.ToTable("Return");

                entity.HasIndex(e => e.ReturnNumber)
                    .IsUnique();

                entity.HasIndex(e => e.ReturnStatusCode);

                entity.Property(e => e.ReturnId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.ReturnNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ReturnStatusCodeNavigation)
                    .WithMany(p => p.Returns)
                    .HasForeignKey(d => d.ReturnStatusCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReturnEvent>(entity =>
            {
                entity.ToTable("ReturnEvent");

                entity.HasIndex(e => e.ReturnTransactionId);

                entity.Property(e => e.ReturnEventId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.ReturnTransaction)
                    .WithMany(p => p.ReturnEvents)
                    .HasForeignKey(d => d.ReturnTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReturnItem>(entity =>
            {
                entity.ToTable("ReturnItem");

                entity.HasIndex(e => e.ReturnId);

                entity.HasIndex(e => e.ReturnRequestItemId);

                entity.Property(e => e.ReturnItemId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnItemSequence])");

                entity.HasOne(d => d.Return)
                    .WithMany(p => p.ReturnItems)
                    .HasForeignKey(d => d.ReturnId);

                entity.HasOne(d => d.ReturnRequestItem)
                    .WithMany(p => p.ReturnItems)
                    .HasForeignKey(d => d.ReturnRequestItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReturnRequest>(entity =>
            {
                entity.ToTable("ReturnRequest");

                entity.HasIndex(e => e.ReturnRequestNumber)
                    .IsUnique();

                entity.HasIndex(e => e.ReturnRequestReasonCode);

                entity.HasIndex(e => e.ReturnRequestStatusCode);

                entity.HasIndex(e => e.ReturnRequestTypeCode);

                entity.Property(e => e.ReturnRequestId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnRequestSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.ReturnRequestNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnRequestReasonCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnRequestStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnRequestStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.ReturnRequestTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ReturnRequestReasonCodeNavigation)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.ReturnRequestReasonCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ReturnRequestStatusCodeNavigation)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.ReturnRequestStatusCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ReturnRequestTypeCodeNavigation)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.ReturnRequestTypeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReturnRequestEvent>(entity =>
            {
                entity.ToTable("ReturnRequestEvent");

                entity.HasIndex(e => e.ReturnRequestTransactionId);

                entity.Property(e => e.ReturnRequestEventId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnRequestEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.ReturnRequestTransaction)
                    .WithMany(p => p.ReturnRequestEvents)
                    .HasForeignKey(d => d.ReturnRequestTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReturnRequestItem>(entity =>
            {
                entity.ToTable("ReturnRequestItem");

                entity.HasIndex(e => e.FulfillableItemId);

                entity.HasIndex(e => e.ReturnRequestId);

                entity.Property(e => e.ReturnRequestItemId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnRequestItemSequence])");

                entity.HasOne(d => d.FulfillableItem)
                    .WithMany(p => p.ReturnRequestItems)
                    .HasForeignKey(d => d.FulfillableItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ReturnRequest)
                    .WithMany(p => p.ReturnRequestItems)
                    .HasForeignKey(d => d.ReturnRequestId);
            });

            modelBuilder.Entity<ReturnRequestReason>(entity =>
            {
                entity.HasKey(e => e.ReturnRequestReasonCode);

                entity.ToTable("ReturnRequestReason");

                entity.Property(e => e.ReturnRequestReasonCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReturnRequestStatusType>(entity =>
            {
                entity.HasKey(e => e.ReturnRequestStatusCode);

                entity.ToTable("ReturnRequestStatusType");

                entity.Property(e => e.ReturnRequestStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReturnRequestSummaryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReturnRequestSummaryView");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FulfillableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FulfillableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnRequestNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnRequestStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnRequestStatusDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReturnRequestToReturnView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReturnRequestToReturnView");
            });

            modelBuilder.Entity<ReturnRequestTransaction>(entity =>
            {
                entity.ToTable("ReturnRequestTransaction");

                entity.HasIndex(e => e.ReturnRequestId);

                entity.Property(e => e.ReturnRequestTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnRequestTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ReturnRequestStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ReturnRequest)
                    .WithMany(p => p.ReturnRequestTransactions)
                    .HasForeignKey(d => d.ReturnRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReturnRequestType>(entity =>
            {
                entity.HasKey(e => e.ReturnRequestTypeCode);

                entity.ToTable("ReturnRequestType");

                entity.Property(e => e.ReturnRequestTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReturnStatusType>(entity =>
            {
                entity.HasKey(e => e.ReturnStatusCode);

                entity.ToTable("ReturnStatusType");

                entity.Property(e => e.ReturnStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReturnSummaryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReturnSummaryView");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FulfillableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FulfillableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnStatusDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReturnTransaction>(entity =>
            {
                entity.ToTable("ReturnTransaction");

                entity.HasIndex(e => e.ReturnId);

                entity.Property(e => e.ReturnTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [ReturnTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ReturnStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Return)
                    .WithMany(p => p.ReturnTransactions)
                    .HasForeignKey(d => d.ReturnId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("Shipment");

                entity.HasIndex(e => e.ShipmentNumber)
                    .IsUnique();

                entity.HasIndex(e => e.ShipmentStatusCode);

                entity.HasIndex(e => e.ShippingVendorId);

                entity.Property(e => e.ShipmentId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.ShipmentDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.ShipmentNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.ShippingVendorId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TrackingCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ShipmentStatusCodeNavigation)
                    .WithMany(p => p.Shipments)
                    .HasForeignKey(d => d.ShipmentStatusCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ShippingVendor)
                    .WithMany(p => p.Shipments)
                    .HasForeignKey(d => d.ShippingVendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentAddress>(entity =>
            {
                entity.HasKey(e => e.ShipmentId);

                entity.ToTable("ShipmentAddress");

                entity.Property(e => e.ShipmentId).ValueGeneratedNever();

                entity.Property(e => e.ShipToAddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToAddressLine2).HasMaxLength(100);

                entity.Property(e => e.ShipToCity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShipToCountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.ShipToName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToPostalCode)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.ShipToStateCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Shipment)
                    .WithOne(p => p.ShipmentAddress)
                    .HasForeignKey<ShipmentAddress>(d => d.ShipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentEvent>(entity =>
            {
                entity.ToTable("ShipmentEvent");

                entity.HasIndex(e => e.ShipmentTransactionId);

                entity.Property(e => e.ShipmentEventId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.ShipmentTransaction)
                    .WithMany(p => p.ShipmentEvents)
                    .HasForeignKey(d => d.ShipmentTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentItem>(entity =>
            {
                entity.ToTable("ShipmentItem");

                entity.HasIndex(e => e.ShipmentId);

                entity.HasIndex(e => e.ShipmentRequestItemId);

                entity.Property(e => e.ShipmentItemId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentItemSequence])");

                entity.HasOne(d => d.Shipment)
                    .WithMany(p => p.ShipmentItems)
                    .HasForeignKey(d => d.ShipmentId);

                entity.HasOne(d => d.ShipmentRequestItem)
                    .WithMany(p => p.ShipmentItems)
                    .HasForeignKey(d => d.ShipmentRequestItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentRequest>(entity =>
            {
                entity.ToTable("ShipmentRequest");

                entity.HasIndex(e => e.ShipmentRequestNumber)
                    .IsUnique();

                entity.HasIndex(e => e.ShipmentRequestStatusCode);

                entity.Property(e => e.ShipmentRequestId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentRequestSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.ShipmentRequestNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentRequestStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentRequestStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ShipmentRequestStatusCodeNavigation)
                    .WithMany(p => p.ShipmentRequests)
                    .HasForeignKey(d => d.ShipmentRequestStatusCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentRequestAddress>(entity =>
            {
                entity.HasKey(e => e.ShipmentRequestId);

                entity.ToTable("ShipmentRequestAddress");

                entity.Property(e => e.ShipmentRequestId).ValueGeneratedNever();

                entity.Property(e => e.ShipToAddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToAddressLine2).HasMaxLength(100);

                entity.Property(e => e.ShipToCity)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShipToCountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.ShipToName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShipToPostalCode)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.ShipToStateCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.HasOne(d => d.ShipmentRequest)
                    .WithOne(p => p.ShipmentRequestAddress)
                    .HasForeignKey<ShipmentRequestAddress>(d => d.ShipmentRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentRequestEvent>(entity =>
            {
                entity.ToTable("ShipmentRequestEvent");

                entity.HasIndex(e => e.ShipmentRequestTransactionId);

                entity.Property(e => e.ShipmentRequestEventId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentRequestEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.ShipmentRequestTransaction)
                    .WithMany(p => p.ShipmentRequestEvents)
                    .HasForeignKey(d => d.ShipmentRequestTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentRequestItem>(entity =>
            {
                entity.ToTable("ShipmentRequestItem");

                entity.HasIndex(e => e.FulfillableItemId);

                entity.HasIndex(e => e.ShipmentRequestId);

                entity.Property(e => e.ShipmentRequestItemId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentRequestItemSequence])");

                entity.HasOne(d => d.FulfillableItem)
                    .WithMany(p => p.ShipmentRequestItems)
                    .HasForeignKey(d => d.FulfillableItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ShipmentRequest)
                    .WithMany(p => p.ShipmentRequestItems)
                    .HasForeignKey(d => d.ShipmentRequestId);
            });

            modelBuilder.Entity<ShipmentRequestStatusType>(entity =>
            {
                entity.HasKey(e => e.ShipmentRequestStatusCode);

                entity.ToTable("ShipmentRequestStatusType");

                entity.Property(e => e.ShipmentRequestStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShipmentRequestSummaryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ShipmentRequestSummaryView");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FulfillableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FulfillableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentRequestNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentRequestStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentRequestStatusDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShipmentRequestToShipmentView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ShipmentRequestToShipmentView");
            });

            modelBuilder.Entity<ShipmentRequestTransaction>(entity =>
            {
                entity.ToTable("ShipmentRequestTransaction");

                entity.HasIndex(e => e.ShipmentRequestId);

                entity.Property(e => e.ShipmentRequestTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentRequestTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ShipmentRequestStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ShipmentRequest)
                    .WithMany(p => p.ShipmentRequestTransactions)
                    .HasForeignKey(d => d.ShipmentRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShipmentStatusType>(entity =>
            {
                entity.HasKey(e => e.ShipmentStatusCode);

                entity.ToTable("ShipmentStatusType");

                entity.Property(e => e.ShipmentStatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShipmentSummaryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ShipmentSummaryView");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.FulfillableName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FulfillableReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShipmentStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.ShippingVendorId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TrackingCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShipmentTransaction>(entity =>
            {
                entity.ToTable("ShipmentTransaction");

                entity.HasIndex(e => e.ShipmentId);

                entity.Property(e => e.ShipmentTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [ShipmentTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ShipmentStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Shipment)
                    .WithMany(p => p.ShipmentTransactions)
                    .HasForeignKey(d => d.ShipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ShippingVendor>(entity =>
            {
                entity.ToTable("ShippingVendor");

                entity.Property(e => e.ShippingVendorId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SquareCustomer>(entity =>
            {
                entity.ToTable("SquareCustomer");

                entity.HasIndex(e => e.SquareCustomerReference)
                    .IsUnique();

                entity.Property(e => e.SquareCustomerId).HasDefaultValueSql("(NEXT VALUE FOR [SquareCustomerSequence])");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquareCustomerReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<SquarePayload>(entity =>
            {
                entity.ToTable("SquarePayload");

                entity.HasIndex(e => e.SquareWebPaymentRequestId);

                entity.Property(e => e.SquarePayloadId).HasDefaultValueSql("(NEXT VALUE FOR [SquarePayloadSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.PayloadJson).IsRequired();

                entity.Property(e => e.PayloadTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.SquareWebPaymentRequest)
                    .WithMany(p => p.SquarePayloads)
                    .HasForeignKey(d => d.SquareWebPaymentRequestId);
            });

            modelBuilder.Entity<SquarePayment>(entity =>
            {
                entity.ToTable("SquarePayment");

                entity.HasIndex(e => e.SquareCustomerId);

                entity.HasIndex(e => e.SquarePaymentRecordId)
                    .IsUnique()
                    .HasFilter("([SquarePaymentRecordId] IS NOT NULL)");

                entity.HasIndex(e => e.SquarePaymentReference)
                    .IsUnique();

                entity.Property(e => e.SquarePaymentId).HasDefaultValueSql("(NEXT VALUE FOR [SquarePaymentSequence])");

                entity.Property(e => e.PaymentAmount).HasColumnType("money");

                entity.Property(e => e.ProcessingFeeAmount).HasColumnType("money");

                entity.Property(e => e.RefundAmount).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquarePaymentRecordId)
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.SquarePaymentReference)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.SquareCustomer)
                    .WithMany(p => p.SquarePayments)
                    .HasForeignKey(d => d.SquareCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquarePaymentEvent>(entity =>
            {
                entity.ToTable("SquarePaymentEvent");

                entity.HasIndex(e => e.SquarePaymentTransactionId);

                entity.Property(e => e.SquarePaymentEventId).HasDefaultValueSql("(NEXT VALUE FOR [SquarePaymentEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.SquarePaymentTransaction)
                    .WithMany(p => p.SquarePaymentEvents)
                    .HasForeignKey(d => d.SquarePaymentTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquarePaymentPayload>(entity =>
            {
                entity.HasKey(e => e.SquarePayloadId);

                entity.ToTable("SquarePaymentPayload");

                entity.HasIndex(e => new { e.SquarePaymentRecordId, e.VersionNumber })
                    .IsUnique();

                entity.Property(e => e.SquarePayloadId).ValueGeneratedNever();

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.PaymentPayloadJson).IsRequired();

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquarePaymentRecordId)
                    .IsRequired()
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.SquarePayload)
                    .WithOne(p => p.SquarePaymentPayload)
                    .HasForeignKey<SquarePaymentPayload>(d => d.SquarePayloadId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquarePaymentTransaction>(entity =>
            {
                entity.ToTable("SquarePaymentTransaction");

                entity.HasIndex(e => e.SquarePayloadId);

                entity.HasIndex(e => e.SquarePaymentId);

                entity.HasIndex(e => new { e.SquarePaymentRecordId, e.VersionNumber })
                    .IsUnique();

                entity.Property(e => e.SquarePaymentTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [SquarePaymentTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.PaymentAmount).HasColumnType("money");

                entity.Property(e => e.ProcessingFeeAmount).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquarePaymentRecordId)
                    .IsRequired()
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.SquarePayload)
                    .WithMany(p => p.SquarePaymentTransactions)
                    .HasForeignKey(d => d.SquarePayloadId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SquarePayment)
                    .WithMany(p => p.SquarePaymentTransactions)
                    .HasForeignKey(d => d.SquarePaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquareRefund>(entity =>
            {
                entity.ToTable("SquareRefund");

                entity.HasIndex(e => e.SquarePaymentId);

                entity.HasIndex(e => e.SquareRefundRecordId)
                    .IsUnique();

                entity.Property(e => e.SquareRefundId).HasDefaultValueSql("(NEXT VALUE FOR [SquareRefundSequence])");

                entity.Property(e => e.ProcessingFeeAmount).HasColumnType("money");

                entity.Property(e => e.RefundAmount).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquareRefundRecordId)
                    .IsRequired()
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.SquarePayment)
                    .WithMany(p => p.SquareRefunds)
                    .HasForeignKey(d => d.SquarePaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquareRefundEvent>(entity =>
            {
                entity.ToTable("SquareRefundEvent");

                entity.HasIndex(e => e.SquareRefundTransactionId);

                entity.Property(e => e.SquareRefundEventId).HasDefaultValueSql("(NEXT VALUE FOR [SquareRefundEventSequence])");

                entity.Property(e => e.EventDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.SquareRefundTransaction)
                    .WithMany(p => p.SquareRefundEvents)
                    .HasForeignKey(d => d.SquareRefundTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquareRefundPayload>(entity =>
            {
                entity.HasKey(e => e.SquarePayloadId);

                entity.ToTable("SquareRefundPayload");

                entity.HasIndex(e => new { e.SquareRefundRecordId, e.VersionNumber })
                    .IsUnique();

                entity.Property(e => e.SquarePayloadId).ValueGeneratedNever();

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RefundPayloadJson).IsRequired();

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquarePaymentRecordId)
                    .IsRequired()
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.SquareRefundRecordId)
                    .IsRequired()
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.SquarePayload)
                    .WithOne(p => p.SquareRefundPayload)
                    .HasForeignKey<SquareRefundPayload>(d => d.SquarePayloadId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquareRefundTransaction>(entity =>
            {
                entity.ToTable("SquareRefundTransaction");

                entity.HasIndex(e => e.SquarePayloadId);

                entity.HasIndex(e => e.SquareRefundId);

                entity.HasIndex(e => new { e.SquareRefundRecordId, e.VersionNumber })
                    .IsUnique();

                entity.Property(e => e.SquareRefundTransactionId).HasDefaultValueSql("(NEXT VALUE FOR [SquareRefundTransactionSequence])");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ProcessingFeeAmount).HasColumnType("money");

                entity.Property(e => e.RefundAmount).HasColumnType("money");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.SquareRefundRecordId)
                    .IsRequired()
                    .HasMaxLength(192)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.UnitOfWork)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.SquarePayload)
                    .WithMany(p => p.SquareRefundTransactions)
                    .HasForeignKey(d => d.SquarePayloadId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SquareRefund)
                    .WithMany(p => p.SquareRefundTransactions)
                    .HasForeignKey(d => d.SquareRefundId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SquareWebPaymentRequest>(entity =>
            {
                entity.ToTable("SquareWebPaymentRequest");

                entity.HasIndex(e => e.SquarePaymentId);

                entity.Property(e => e.SquareWebPaymentRequestId).HasDefaultValueSql("(NEXT VALUE FOR [SquareWebPaymentRequestSequence])");

                entity.Property(e => e.IndempotencyKey)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Nonce)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ProcessingStatusCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessingStatusDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.RequestJson)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.TransactionAmount).HasColumnType("money");

                entity.Property(e => e.UpdateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.WebRequestTypeCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.SquarePayment)
                    .WithMany(p => p.SquareWebPaymentRequests)
                    .HasForeignKey(d => d.SquarePaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.StateCode, e.CountryCode });

                entity.ToTable("State");

                entity.HasIndex(e => e.CountryCode);

                entity.Property(e => e.StateCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CountryCodeNavigation)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.CountryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag");

                entity.HasIndex(e => e.TagTypeCode);

                entity.Property(e => e.TagId).HasDefaultValueSql("(NEXT VALUE FOR [TagSequence])");

                entity.Property(e => e.CreateDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.TagTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.TagTypeCodeNavigation)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(d => d.TagTypeCode);
            });

            modelBuilder.Entity<TagCategory>(entity =>
            {
                entity.HasKey(e => e.TagCategoryCode);

                entity.ToTable("TagCategory");

                entity.Property(e => e.TagCategoryCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TagType>(entity =>
            {
                entity.HasKey(e => e.TagTypeCode);

                entity.ToTable("TagType");

                entity.HasIndex(e => e.TagCategoryCode);

                entity.Property(e => e.TagTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TagCategoryCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.TagCategoryCodeNavigation)
                    .WithMany(p => p.TagTypes)
                    .HasForeignKey(d => d.TagCategoryCode);
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("Topic");

                entity.HasIndex(e => e.TopicReference)
                    .IsUnique();

                entity.Property(e => e.TopicId).HasDefaultValueSql("(NEXT VALUE FOR [TopicSequence])");

                entity.Property(e => e.TopicReference).IsRequired();
            });

            modelBuilder.Entity<TopicField>(entity =>
            {
                entity.HasKey(e => new { e.TopicId, e.FieldCode });

                entity.ToTable("TopicField");

                entity.Property(e => e.FieldCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FieldValue)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.TopicFields)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UnitOfMeasure>(entity =>
            {
                entity.HasKey(e => e.UnitOfMeasureCode);

                entity.ToTable("UnitOfMeasure");

                entity.Property(e => e.UnitOfMeasureCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.ToTable("UserAddress");

                entity.HasIndex(e => e.AddressTypeCode);

                entity.HasIndex(e => new { e.StateCode, e.CountryCode });

                entity.HasIndex(e => e.UserProfileId);

                entity.Property(e => e.UserAddressId).HasDefaultValueSql("(NEXT VALUE FOR [UserAddressSequence])");

                entity.Property(e => e.AddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AddressLine2).HasMaxLength(100);

                entity.Property(e => e.AddressTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.Property(e => e.StateCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.AddressTypeCodeNavigation)
                    .WithMany(p => p.UserAddresses)
                    .HasForeignKey(d => d.AddressTypeCode);

                entity.HasOne(d => d.UserProfile)
                    .WithMany(p => p.UserAddresses)
                    .HasForeignKey(d => d.UserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.State)
                    .WithMany(p => p.UserAddresses)
                    .HasForeignKey(d => new { d.StateCode, d.CountryCode })
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfile");

                entity.HasIndex(e => e.UserProfileReference)
                    .IsUnique();

                entity.Property(e => e.UserProfileId).HasDefaultValueSql("(NEXT VALUE FOR [UserProfileSequence])");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Nickname).HasMaxLength(50);

                entity.Property(e => e.TimeZoneId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserProfileReference).IsRequired();

                entity.Property(e => e.WebsiteUrl).HasMaxLength(1000);
            });

            modelBuilder.Entity<UserProfileAspNetUser>(entity =>
            {
                entity.HasKey(e => e.UserProfileId);

                entity.ToTable("UserProfileAspNetUser");

                entity.HasIndex(e => e.AspNetUserId);

                entity.Property(e => e.UserProfileId).ValueGeneratedNever();

                entity.Property(e => e.AspNetUserId).IsRequired();

                entity.HasOne(d => d.AspNetUser)
                    .WithMany(p => p.UserProfileAspNetUsers)
                    .HasForeignKey(d => d.AspNetUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserProfile)
                    .WithOne(p => p.UserProfileAspNetUser)
                    .HasForeignKey<UserProfileAspNetUser>(d => d.UserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserProperty>(entity =>
            {
                entity.ToTable("UserProperty");

                entity.HasIndex(e => e.UserProfileId);

                entity.Property(e => e.UserPropertyId).HasDefaultValueSql("(NEXT VALUE FOR [UserPropertySequence])");

                entity.Property(e => e.PropertyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PropertyValue)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserProfile)
                    .WithMany(p => p.UserProperties)
                    .HasForeignKey(d => d.UserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("Vendor");

                entity.Property(e => e.VendorId).HasDefaultValueSql("(NEXT VALUE FOR [VendorSequence])");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WebsiteProperty>(entity =>
            {
                entity.HasKey(e => e.PropertyName);

                entity.ToTable("WebsiteProperty");

                entity.Property(e => e.PropertyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.HasSequence("AlertEmailRequestSequence").HasMin(1);

            modelBuilder.HasSequence("AlertSequence").HasMin(1);

            modelBuilder.HasSequence("ArtifactSequence").HasMin(1);

            modelBuilder.HasSequence("AspNetRoleClaimsSequence").HasMin(1);

            modelBuilder.HasSequence("AspNetUserClaimsSequence").HasMin(1);

            modelBuilder.HasSequence("ConsumableSequence").HasMin(1);

            modelBuilder.HasSequence("DesignSnapshotSequence").HasMin(1);

            modelBuilder.HasSequence("EmailRequestSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableAddressSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableEventSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableItemComponentSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableItemSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableTransactionItemSequence").HasMin(1);

            modelBuilder.HasSequence("FulfillableTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("FundableEventSequence").HasMin(1);

            modelBuilder.HasSequence("FundableSequence").HasMin(1);

            modelBuilder.HasSequence("FundableTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("FunderEventSequence").HasMin(1);

            modelBuilder.HasSequence("FunderSequence").HasMin(1);

            modelBuilder.HasSequence("FunderTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("FundingEventSequence").HasMin(1);

            modelBuilder.HasSequence("FundingTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("InventoryItemSequence").HasMin(1);

            modelBuilder.HasSequence("InventoryItemStockSequence").HasMin(1);

            modelBuilder.HasSequence("InventoryItemStockTransactionItemSequence").HasMin(1);

            modelBuilder.HasSequence("InventoryItemStockTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("InventoryItemTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("KansasTaxTableEntrySequence").HasMin(1);

            modelBuilder.HasSequence("KansasTaxTableSequence").HasMin(1);

            modelBuilder.HasSequence("LedgerAccountSubtotalSequence").HasMin(1);

            modelBuilder.HasSequence("LedgerTransactionEntrySequence");

            modelBuilder.HasSequence("LedgerTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("LogEntrySequence").HasMin(1);

            modelBuilder.HasSequence("MessageEmailRequestSequence").HasMin(1);

            modelBuilder.HasSequence("MessageSequence").HasMin(1);

            modelBuilder.HasSequence("NotificationEmailRequestSequence").HasMin(1);

            modelBuilder.HasSequence("NotificationSequence").HasMin(1);

            modelBuilder.HasSequence("OrderableComponentSequence").HasMin(1);

            modelBuilder.HasSequence("OrderableSequence").HasMin(1);

            modelBuilder.HasSequence("OrdererSequence").HasMin(1);

            modelBuilder.HasSequence("OrderEventSequence").HasMin(1);

            modelBuilder.HasSequence("OrderItemSequence").HasMin(1);

            modelBuilder.HasSequence("OrderSequence").HasMin(1);

            modelBuilder.HasSequence("OrderTransactionItemSequence").HasMin(1);

            modelBuilder.HasSequence("OrderTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("OwnerSequence").HasMin(1);

            modelBuilder.HasSequence("ParticipantSequence").HasMin(1);

            modelBuilder.HasSequence("PayPalIpnProcessLogSequence").HasMin(1);

            modelBuilder.HasSequence("PayPalIpnSequence").HasMin(1);

            modelBuilder.HasSequence("PayPalOrderResponseSequence").HasMin(1);

            modelBuilder.HasSequence("PricingScheduleEntrySequence").HasMin(1);

            modelBuilder.HasSequence("PricingScheduleSequence").HasMin(1);

            modelBuilder.HasSequence("ProjectSnapshotComponentSequence").HasMin(1);

            modelBuilder.HasSequence("ProjectSnapshotSequence").HasMin(1);

            modelBuilder.HasSequence("ReportInstanceSequence").HasMin(1);

            modelBuilder.HasSequence("ResourceLibrarySequence").HasMin(1);

            modelBuilder.HasSequence("ResourceSequence").HasMin(1);

            modelBuilder.HasSequence("ResourceTypeSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnEventSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnItemSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnRequestEventSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnRequestItemSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnRequestSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnRequestTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnSequence").HasMin(1);

            modelBuilder.HasSequence("ReturnTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentEventSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentItemSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentRequestEventSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentRequestItemSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentRequestSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentRequestTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentSequence").HasMin(1);

            modelBuilder.HasSequence("ShipmentTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("SquareCustomerSequence").HasMin(1);

            modelBuilder.HasSequence("SquarePayloadSequence").HasMin(1);

            modelBuilder.HasSequence("SquarePaymentEventSequence").HasMin(1);

            modelBuilder.HasSequence("SquarePaymentSequence").HasMin(1);

            modelBuilder.HasSequence("SquarePaymentTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("SquareRefundEventSequence").HasMin(1);

            modelBuilder.HasSequence("SquareRefundSequence").HasMin(1);

            modelBuilder.HasSequence("SquareRefundTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("SquareWebhookPayloadSequence").HasMin(1);

            modelBuilder.HasSequence("SquareWebhookTransactionSequence");

            modelBuilder.HasSequence("SquareWebPaymentRequestSequence").HasMin(1);

            modelBuilder.HasSequence("SquareWebPaymentSequence").HasMin(1);

            modelBuilder.HasSequence("SquareWebPaymentTransactionSequence").HasMin(1);

            modelBuilder.HasSequence("sysdiagramsSequence").HasMin(1);

            modelBuilder.HasSequence("TagSequence").HasMin(1);

            modelBuilder.HasSequence("TopicSequence").HasMin(1);

            modelBuilder.HasSequence("UserAddressSequence").HasMin(1);

            modelBuilder.HasSequence("UserProfileSequence").HasMin(1);

            modelBuilder.HasSequence("UserPropertySequence").HasMin(1);

            modelBuilder.HasSequence("VendorSequence").HasMin(1);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
