using CRM.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Database;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ContactSalutation> ContactSalutations { get; set; }

    public virtual DbSet<ContactType> ContactTypes { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractItem> ContractItems { get; set; }

    public virtual DbSet<ContractStage> ContractStages { get; set; }

    public virtual DbSet<ContractType> ContractTypes { get; set; }

    public virtual DbSet<ContractDocument> ContractDocuments { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerContact> CustomerContacts { get; set; }

    public virtual DbSet<CustomerType> CustomerTypes { get; set; }

    public virtual DbSet<Deposit> Deposits { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeLevel> EmployeeLevels { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<InstallmentSchedule> InstallmentSchedules { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<LeadItem> LeadItems { get; set; }

    public virtual DbSet<LeadPotentialLevel> LeadPotentialLevels { get; set; }

    public virtual DbSet<LeadSource> LeadSources { get; set; }

    public virtual DbSet<LeadStage> LeadStages { get; set; }

    public virtual DbSet<Opportunity> Opportunities { get; set; }

    public virtual DbSet<OpportunityItem> OpportunityItems { get; set; }

    public virtual DbSet<OpportunityStage> OpportunityStages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentOption> PaymentOptions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Project> Projects { get; set; }


    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        => optionsBuilder.UseSqlServer("Server=.;Database=FLC_CRM_2;User Id=sa;Password=23102001;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__account__46A222CD5CDE738C");

            entity.ToTable("account", tb => tb.HasTrigger("trg_generate_account_code"));

            entity.HasIndex(e => e.AccountName, "UQ__account__6894C54A60BF4609").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AccountCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("account_code");
            entity.Property(e => e.AccountDescription)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("account_description");
            entity.Property(e => e.AccountName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("account_name");
            entity.Property(e => e.AccountTypeId).HasColumnName("account_type_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");

            entity.HasOne(d => d.AccountType).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccountTypeId)
                .HasConstraintName("fk_account_type");

            entity.HasOne(d => d.Employee).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_account_employee");
        });

        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.AccountTypeId).HasName("PK__account___18186C1005C3471B");

            entity.ToTable("account_type");

            entity.HasIndex(e => e.AccountTypeCode, "UQ__account___72584A925BFECCC9").IsUnique();

            entity.Property(e => e.AccountTypeId)
                .ValueGeneratedNever()
                .HasColumnName("account_type_id");
            entity.Property(e => e.AccountTypeCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("account_type_code");
            entity.Property(e => e.AccountTypeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("account_type_name");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__contact__024E7A865A3BC2F9");

            entity.ToTable("contact");

            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.ContactAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contact_address");
            entity.Property(e => e.ContactDescription)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("contact_description");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactName)
                .HasMaxLength(255)
                .HasColumnName("contact_name");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contact_phone");
            entity.Property(e => e.ContactSalutationId).HasColumnName("contact_salutation_id");
            entity.Property(e => e.ContactTypeId).HasColumnName("contact_type_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");

            entity.HasOne(d => d.ContactSalutation).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.ContactSalutationId)
                .HasConstraintName("fk_contact_salutation");

            entity.HasOne(d => d.ContactType).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.ContactTypeId)
                .HasConstraintName("fk_contact_type");
        });

        modelBuilder.Entity<ContactSalutation>(entity =>
        {
            entity.HasKey(e => e.ContactSalutationId).HasName("PK__contact___9150F11019B4E364");

            entity.ToTable("contact_salutation");

            entity.HasIndex(e => e.ContactSalutationCode, "UQ__contact___9D12EB252B505BE9").IsUnique();

            entity.Property(e => e.ContactSalutationId)
                .ValueGeneratedNever()
                .HasColumnName("contact_salutation_id");
            entity.Property(e => e.ContactSalutationCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contact_salutation_code");
            entity.Property(e => e.ContactSalutationName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contact_salutation_name");
        });

        modelBuilder.Entity<ContactType>(entity =>
        {
            entity.HasKey(e => e.ContactTypeId).HasName("PK__contact___3A81B3272D8C2C1E");

            entity.ToTable("contact_type");

            entity.HasIndex(e => e.ContactTypeCode, "UQ__contact___D4C9E3B2E2E2F6C3").IsUnique();

            entity.Property(e => e.ContactTypeId)
                .ValueGeneratedNever()
                .HasColumnName("contact_type_id");
            entity.Property(e => e.ContactTypeCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_type_code");
            entity.Property(e => e.ContactTypeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contact_type_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__contract__F8D66423B14F7943");

            entity.ToTable("contract", tb => tb.HasTrigger("trg_generate_contract_code"));

            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.ContractCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contract_code");
            entity.Property(e => e.ContractDescription).HasColumnName("contract_description");
            entity.Property(e => e.ContractEndDate).HasColumnName("contract_end_date");
            entity.Property(e => e.ContractName)
                .HasMaxLength(255)
                .HasColumnName("contract_name");
            entity.Property(e => e.ContractStageId).HasColumnName("contract_stage_id");
            entity.Property(e => e.ContractStartDate).HasColumnName("contract_start_date");
            entity.Property(e => e.ContractTypeId).HasColumnName("contract_type_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DaysRemaining)
                .HasComputedColumnSql("(datediff(day,getdate(),[contract_end_date]))", false)
                .HasColumnName("days_remaining");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");

            entity.HasOne(d => d.ContractStage).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ContractStageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_stage");

            entity.HasOne(d => d.ContractType).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ContractTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_type");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_employee");
        });

        modelBuilder.Entity<ContractItem>(entity =>
        {
            entity.HasKey(e => e.ContractItemId).HasName("PK__contract__2BA8AB28E5BF6019");

            entity.ToTable("contract_item");

            entity.Property(e => e.ContractItemId)
                .ValueGeneratedNever()
                .HasColumnName("contract_item_id");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.CostTax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("cost_tax");
            entity.Property(e => e.GrandTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("grand_total");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SalePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sale_price");

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractItems)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_citem_contract");

            entity.HasOne(d => d.Product).WithMany(p => p.ContractItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_citem_product");
        });

        modelBuilder.Entity<ContractStage>(entity =>
        {
            entity.HasKey(e => e.ContractStageId).HasName("PK__contract__497F563BB890CF74");

            entity.ToTable("contract_stage");

            entity.HasIndex(e => e.ContractStageCode, "UQ__contract__0A4D9136E656C356").IsUnique();

            entity.Property(e => e.ContractStageId)
                .ValueGeneratedNever()
                .HasColumnName("contract_stage_id");
            entity.Property(e => e.ContractStageCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contract_stage_code");
            entity.Property(e => e.ContractStageName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contract_stage_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<ContractType>(entity =>
        {
            entity.HasKey(e => e.ContractTypeId).HasName("PK__contract__B66967DF985EED60");

            entity.ToTable("contract_type");

            entity.HasIndex(e => e.ContractTypeCode, "UQ__contract__E404FEBA8221DE44").IsUnique();

            entity.Property(e => e.ContractTypeId)
                .ValueGeneratedNever()
                .HasColumnName("contract_type_id");
            entity.Property(e => e.ContractTypeCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contract_type_code");
            entity.Property(e => e.ContractTypeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contract_type_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB85F7CC399B");

            entity.ToTable("customer", tb => tb.HasTrigger("trg_generate_customer_code"));

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.CustomerAddress)
                .HasMaxLength(255)
                .HasColumnName("customer_address");
            entity.Property(e => e.CustomerBirthDay).HasColumnName("customer_birth_day");
            entity.Property(e => e.CustomerCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("customer_code");
            entity.Property(e => e.CustomerDescription)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("customer_description");
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("customer_email");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(255)
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerIdentityCard)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("customer_identity_card");
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("customer_phone");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Customers)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_customer_employee");

            entity.HasOne(d => d.Gender).WithMany(p => p.Customers)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("fk_customer_gender");
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.HasKey(e => e.CustomerContactId).HasName("PK__customer__C474D9294CE65D75");

            entity.ToTable("customer_contact");

            entity.Property(e => e.CustomerContactId).HasColumnName("customer_contact_id");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Contact).WithMany(p => p.CustomerContacts)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_contact_contact");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerContacts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_contact_customer");
        });

        modelBuilder.Entity<CustomerType>(entity =>
        {
            entity.HasKey(e => e.CustomerTypeId).HasName("PK__customer__3320C939AF5F6087");

            entity.ToTable("customer_type");

            entity.HasIndex(e => e.CustomerTypeCode, "UQ__customer__9C993D72E755251E").IsUnique();

            entity.Property(e => e.CustomerTypeId)
                .ValueGeneratedNever()
                .HasColumnName("customer_type_id");
            entity.Property(e => e.CustomerTypeCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("customer_type_code");
            entity.Property(e => e.CustomerTypeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("customer_type_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.HasKey(e => e.DepositId).HasName("PK__deposit__4450A62AFD23947B");

            entity.ToTable("deposit", tb => tb.HasTrigger("trg_generate_deposit_code"));

            entity.Property(e => e.DepositId).HasColumnName("deposit_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DepositCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("deposit_code");
            entity.Property(e => e.DepositCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("deposit_cost");
            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("description");
            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("fk_deposit_customer");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.OpportunityId)
                .HasConstraintName("fk_deposit_opportunity");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__employee__C52E0BA8E5913651");

            entity.ToTable("employee", tb => tb.HasTrigger("trg_generate_employee_code"));

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.EmployeeAddress)
                .HasMaxLength(255)
                .HasColumnName("employee_address");
            entity.Property(e => e.EmployeeBirthDay).HasColumnName("employee_birth_day");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("employee_code");
            entity.Property(e => e.EmployeeDescription)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("employee_description");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("employee_email");
            entity.Property(e => e.EmployeeLevelId).HasColumnName("employee_level_id");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(150)
                .HasColumnName("employee_name");
            entity.Property(e => e.EmployeePhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_phone");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");

            entity.HasOne(d => d.EmployeeLevel).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeLevelId)
                .HasConstraintName("fk_employee_level");

            entity.HasOne(d => d.Gender).WithMany(p => p.Employees)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("fk_employee_gender");
        });

        modelBuilder.Entity<EmployeeLevel>(entity =>
        {
            entity.HasKey(e => e.EmployeeLevelId).HasName("PK__employee__5FAC047D0FE5CA9B");

            entity.ToTable("employee_level");

            entity.HasIndex(e => e.EmployeeLevelCode, "UQ__employee__8E85B61F02F868A4").IsUnique();

            entity.Property(e => e.EmployeeLevelId)
                .ValueGeneratedNever()
                .HasColumnName("employee_level_id");
            entity.Property(e => e.EmployeeLevelCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("employee_level_code");
            entity.Property(e => e.EmployeeLevelName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("employee_level_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__gender__9DF18F8740D2A275");

            entity.ToTable("gender");

            entity.HasIndex(e => e.GenderCode, "UQ__gender__DD0984216B1A6BD5").IsUnique();

            entity.Property(e => e.GenderId)
                .ValueGeneratedNever()
                .HasColumnName("gender_id");
            entity.Property(e => e.GenderCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gender_code");
            entity.Property(e => e.GenderName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gender_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<InstallmentSchedule>(entity =>
        {
            entity.HasKey(e => e.InstallmentId).HasName("PK__installm__B591CAB10D8F0C3D");

            entity.ToTable("installment_schedule");

            entity.Property(e => e.InstallmentId).HasColumnName("installment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.InstallmentNo).HasColumnName("installment_no");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.Property(e => e.IsDeposited)
                .HasColumnName("is_deposited")
                .HasDefaultValue(false);

            entity.HasOne(d => d.Contract).WithMany(p => p.InstallmentSchedules)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_installment_contract");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__invoice__F58DFD497D919D36");

            entity.ToTable("invoice", tb => tb.HasTrigger("trg_generate_invoice_code"));

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.InvoiceCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("invoice_code");

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(InvoiceStatus.Pending)
                .HasColumnName("status");

            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Contract).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_invoice_contract");

        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.LeadId).HasName("PK__lead__B54D340B1F70EFB1");

            entity.ToTable("lead", tb => tb.HasTrigger("trg_generate_lead_code"));

            entity.Property(e => e.LeadId).HasColumnName("lead_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Daypassed)
                .HasComputedColumnSql("(datediff(day,[create_date],getdate()))", false)
                .HasColumnName("daypassed");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.LeadAddress)
                .HasMaxLength(255)
                .HasColumnName("lead_address");
            entity.Property(e => e.LeadCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("lead_code");
            entity.Property(e => e.LeadDescription).HasColumnName("lead_description");
            entity.Property(e => e.LeadEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("lead_email");
            entity.Property(e => e.LeadName)
                .HasMaxLength(255)
                .HasColumnName("lead_name");
            entity.Property(e => e.LeadPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lead_phone");
            entity.Property(e => e.LeadPotentialLevelId).HasColumnName("lead_potential_level_id");
            entity.Property(e => e.LeadSourceId).HasColumnName("lead_source_id");
            entity.Property(e => e.LeadStageId).HasColumnName("lead_stage_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Leads)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_lead_employee");

            entity.HasOne(d => d.LeadPotentialLevel).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadPotentialLevelId)
                .HasConstraintName("fk_lead_potential");

            entity.HasOne(d => d.LeadSource).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadSourceId)
                .HasConstraintName("fk_lead_source");

            entity.HasOne(d => d.LeadStage).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadStageId)
                .HasConstraintName("fk_lead_stage");
        });

        modelBuilder.Entity<LeadItem>(entity =>
        {
            entity.HasKey(e => e.LeadProductId).HasName("PK__lead_ite__8100A3806102DEA9");

            entity.ToTable("lead_item");

            entity.Property(e => e.LeadProductId).HasColumnName("lead_product_id");
            entity.Property(e => e.LeadId).HasColumnName("lead_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadItems)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_leaditem_lead");

            entity.HasOne(d => d.Product).WithMany(p => p.LeadItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_leaditem_product");
        });

        modelBuilder.Entity<LeadPotentialLevel>(entity =>
        {
            entity.HasKey(e => e.LeadPotentialLevelId).HasName("PK__lead_pot__6827B4C11739CE98");

            entity.ToTable("lead_potential_level");

            entity.HasIndex(e => e.LeadPotentialLevelCode, "UQ__lead_pot__53434C60F73BDA88").IsUnique();

            entity.Property(e => e.LeadPotentialLevelId)
                .ValueGeneratedNever()
                .HasColumnName("lead_potential_level_id");
            entity.Property(e => e.LeadPotentialLevelCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("lead_potential_level_code");
            entity.Property(e => e.LeadPotentialLevelName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lead_potential_level_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<LeadSource>(entity =>
        {
            entity.HasKey(e => e.LeadSourceId).HasName("PK__lead_sou__02D65A87980A8D25");

            entity.ToTable("lead_source");

            entity.HasIndex(e => e.LeadSourceCode, "UQ__lead_sou__A5D86DD3C57A1E19").IsUnique();

            entity.Property(e => e.LeadSourceId)
                .ValueGeneratedNever()
                .HasColumnName("lead_source_id");
            entity.Property(e => e.LeadSourceCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("lead_source_code");
            entity.Property(e => e.LeadSourceName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lead_source_name")
                .HasColumnType("nvarchar(255)");
        });

        modelBuilder.Entity<LeadStage>(entity =>
        {
            entity.HasKey(e => e.LeadStageId).HasName("PK__lead_sta__A53C6BB77B6BCA7C");

            entity.ToTable("lead_stage");

            entity.HasIndex(e => e.LeadStageCode, "UQ__lead_sta__30AC28F616E2B8D1").IsUnique();

            entity.Property(e => e.LeadStageId)
                .ValueGeneratedNever()
                .HasColumnName("lead_stage_id");
            entity.Property(e => e.LeadStageCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("lead_stage_code");
            entity.Property(e => e.LeadStageName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lead_stage_name")
            .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Opportunity>(entity =>
        {
            entity.HasKey(e => e.OpportunityId).HasName("PK__opportun__B975AC8A7F8DAB17");

            entity.ToTable("opportunity", tb => tb.HasTrigger("trg_generate_oop_code"));

            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DaysRemaining)
                .HasComputedColumnSql("(datediff(day,getdate(),[end_date]))", false)
                .HasColumnName("days_remaining");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.OpportunityCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("opportunity_code");
            entity.Property(e => e.OpportunityDescription).HasColumnName("opportunity_description");
            entity.Property(e => e.OpportunityName)
                .HasMaxLength(255)
                .HasColumnName("opportunity_name");
            entity.Property(e => e.OpportunityStageId).HasColumnName("opportunity_stage_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_opp_customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_opp_employee");

            entity.HasOne(d => d.OpportunityStage).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.OpportunityStageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_opp_stage");
        });

        modelBuilder.Entity<OpportunityItem>(entity =>
        {
            entity.HasKey(e => e.OpportunityItemId).HasName("PK__opportun__53B445D8027330C6");

            entity.ToTable("opportunity_item");

            entity.Property(e => e.OpportunityItemId).HasColumnName("opportunity_item_id");
            entity.Property(e => e.ExceptedProfit)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("excepted_profit");
            entity.Property(e => e.OpportunityId).HasColumnName("opportunity_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SalePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("sale_price");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.OpportunityItems)
                .HasForeignKey(d => d.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_oppitem_op");

            entity.HasOne(d => d.Product).WithMany(p => p.OpportunityItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_oppitem_product");
        });

        modelBuilder.Entity<OpportunityStage>(entity =>
        {
            entity.HasKey(e => e.OpportunityStageId).HasName("PK__opportun__E987BFFC7B1F3558");

            entity.ToTable("opportunity_stage");

            entity.HasIndex(e => e.OpportunityStageCode, "UQ__opportun__C49CE1D573DD6A67").IsUnique();

            entity.Property(e => e.OpportunityStageId)
                .ValueGeneratedNever()
                .HasColumnName("opportunity_stage_id");
            entity.Property(e => e.OpportunityStageCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("opportunity_stage_code");
            entity.Property(e => e.OpportunityStageName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("opportunity_stage_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payment__ED1FC9EA67F77743");

            entity.ToTable("payment", tb => tb.HasTrigger("trg_payment_insert"));

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.PaymentCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_code");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");

            entity.Property(e => e.RemainAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("remain_amount");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_invoice");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_method");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__payment___8A3EA9EBA2B5D72B");

            entity.ToTable("payment_method");

            entity.HasIndex(e => e.PaymentMethodCode, "UQ__payment___6403475B05AB8EEE").IsUnique();

            entity.Property(e => e.PaymentMethodId)
                .ValueGeneratedNever()
                .HasColumnName("payment_method_id");
            entity.Property(e => e.PaymentMethodCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("payment_method_code");
            entity.Property(e => e.PaymentMethodName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("payment_method_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<PaymentOption>(entity =>
        {
            entity.HasKey(e => e.PaymentOptionId).HasName("PK__payment___331ED182F5DAB06C");

            entity.ToTable("payment_option");

            entity.HasIndex(e => e.PaymentOptionCode, "UQ__payment___41DA7EA88F03517B").IsUnique();

            entity.Property(e => e.PaymentOptionId)
                .ValueGeneratedNever()
                .HasColumnName("payment_option_id");
            entity.Property(e => e.PaymentOptionCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_option_code");
            entity.Property(e => e.PaymentOptionName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("payment_option_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__product__47027DF5BB0B796C");

            entity.ToTable("product", tb => tb.HasTrigger("trg_generate_product_code"));

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("updated_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ProductAddress)
                .HasMaxLength(255)
                .HasColumnName("product_address");
            entity.Property(e => e.ProductArea)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("product_area");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_code");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductNumber).HasColumnName("product_number");
            entity.Property(e => e.ProductPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("product_price");
            entity.Property(e => e.ProductStatusId).HasColumnName("product_status_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.ProductStatus).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_status");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_type");

            entity.HasOne(d => d.Project).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_product_project");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.ProductStatusId).HasName("PK__product___D299CFCF5E40536F");

            entity.ToTable("product_status");

            entity.Property(e => e.ProductStatusId)
                .ValueGeneratedNever()
                .HasColumnName("product_status_id");
            entity.Property(e => e.ProductStatusCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_status_code");
            entity.Property(e => e.ProductStatusName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("product_status_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__product___6EED3ED647063BD9");

            entity.ToTable("product_type");

            entity.Property(e => e.ProductTypeId)
                .ValueGeneratedNever()
                .HasColumnName("product_type_id");
            entity.Property(e => e.ProductTypeCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_type_code");
            entity.Property(e => e.ProductTypeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("product_type_name")
                .HasColumnType("nvarchar");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__project__BC799E1FA2A8F03D");

            entity.ToTable("project", tb => tb.HasTrigger("trg_generate_project_code"));

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.DaysRemaining)
                .HasComputedColumnSql("(case when [project_end_date]>=CONVERT([date],getdate()) then datediff(day,CONVERT([date],getdate()),[project_end_date]) else (0) end)", false)
                .HasColumnName("days_remaining");
            entity.Property(e => e.ProjectAddress)
                .HasMaxLength(255)
                .HasColumnName("project_address");
            entity.Property(e => e.ProjectCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("project_code");
            entity.Property(e => e.ProjectDescription).HasColumnName("project_description");
            entity.Property(e => e.ProjectEndDate).HasColumnName("project_end_date");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .HasColumnName("project_name");
            entity.Property(e => e.ProjectStartDate).HasColumnName("project_start_date");
            entity.Property(e => e.ProjectStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("project_status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
