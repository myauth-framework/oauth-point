﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Mapping;

namespace MyAuth.OAuthPoint.Db
{
	/// <summary>
	/// Database       : myauth-sso
	/// Data Source    : localhost
	/// Server Version : 8.0.26
	/// </summary>
	public partial class MyAuthOAuthPointDb : LinqToDB.Data.DataConnection
	{
		/// <summary>
		/// Client available scopes
		/// </summary>
		public ITable<ClientAvailableScopeDb>  ClientAvailableScopes  { get { return this.GetTable<ClientAvailableScopeDb>(); } }
		/// <summary>
		/// Available redirect URI for client
		/// </summary>
		public ITable<ClientAvailableUriDb>    ClientAvailableUris    { get { return this.GetTable<ClientAvailableUriDb>(); } }
		/// <summary>
		/// Registered clients
		/// </summary>
		public ITable<ClientDb>                Clients                { get { return this.GetTable<ClientDb>(); } }
		/// <summary>
		/// Login sessions
		/// </summary>
		public ITable<LoginSessionDb>          LoginSessions          { get { return this.GetTable<LoginSessionDb>(); } }
		/// <summary>
		/// Additional subject claims for access token
		/// </summary>
		public ITable<SubjectAccessClaimDb>    SubjectAccessClaims    { get { return this.GetTable<SubjectAccessClaimDb>(); } }
		/// <summary>
		/// Subject available scopes
		/// </summary>
		public ITable<SubjectAvailableScopeDb> SubjectAvailableScopes { get { return this.GetTable<SubjectAvailableScopeDb>(); } }
		/// <summary>
		/// Subjects
		/// </summary>
		public ITable<SubjectDb>               Subjects               { get { return this.GetTable<SubjectDb>(); } }
		/// <summary>
		/// Subject claims for identity
		/// </summary>
		public ITable<SubjectIdentityClaimDb>  SubjectIdentityClaims  { get { return this.GetTable<SubjectIdentityClaimDb>(); } }

		public MyAuthOAuthPointDb()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public MyAuthOAuthPointDb(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public MyAuthOAuthPointDb(LinqToDbConnectionOptions options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		public MyAuthOAuthPointDb(LinqToDbConnectionOptions<MyAuthOAuthPointDb> options)
			: base(options)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	/// <summary>
	/// Client available scopes
	/// </summary>
	[Table("client_available_scopes")]
	public partial class ClientAvailableScopeDb
	{
		/// <summary>
		/// Row identifier
		/// </summary>
		[Column("id"),        PrimaryKey, Identity] public int    Id       { get; set; } // int
		/// <summary>
		/// Scope name
		/// </summary>
		[Column("name"),      NotNull             ] public string Name     { get; set; } // varchar(50)
		/// <summary>
		/// Client identifier
		/// </summary>
		[Column("client_id"), NotNull             ] public string ClientId { get; set; } // char(32)

		#region Associations

		/// <summary>
		/// ClientAvailableScopesToClient
		/// </summary>
		[Association(ThisKey="ClientId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="ClientAvailableScopesToClient", BackReferenceName="ClientAvailableScopesToClients")]
		public ClientDb Client { get; set; }

		#endregion
	}

	/// <summary>
	/// Available redirect URI for client
	/// </summary>
	[Table("client_available_uri")]
	public partial class ClientAvailableUriDb
	{
		/// <summary>
		/// Row identifier
		/// </summary>
		[Column("id"),        PrimaryKey, Identity] public int    Id       { get; set; } // int
		/// <summary>
		/// URI
		/// </summary>
		[Column("uri"),       NotNull             ] public string Uri      { get; set; } // varchar(250)
		/// <summary>
		/// Client identifier
		/// </summary>
		[Column("client_id"), NotNull             ] public string ClientId { get; set; } // char(32)

		#region Associations

		/// <summary>
		/// ClientAvailableUriToClient
		/// </summary>
		[Association(ThisKey="ClientId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="ClientAvailableUriToClient", BackReferenceName="ClientAvailableUriToClients")]
		public ClientDb Client { get; set; }

		#endregion
	}

	/// <summary>
	/// Registered clients
	/// </summary>
	[Table("clients")]
	public partial class ClientDb
	{
		/// <summary>
		/// Client identifier (GUID)
		/// </summary>
		[Column("id"),            PrimaryKey,  NotNull] public string                         Id           { get; set; } // char(32)
		/// <summary>
		/// Name
		/// </summary>
		[Column("name"),                       NotNull] public string                         Name         { get; set; } // varchar(50)
		/// <summary>
		/// HEX HMAC-MD5 hash with salt
		/// </summary>
		[Column("password_hash"),              NotNull] public string                         PasswordHash { get; set; } // char(32)
		/// <summary>
		/// Indicated client enabled
		/// </summary>
		[Column("enabled"),                    NotNull] public MyAuth.OAuthPoint.Db.MySqlBool Enabled      { get; set; } // enum('Y','N')
		/// <summary>
		/// &apos;enabled&apos; flag actuallity date time
		/// </summary>
		[Column("enabled_dt"),       Nullable         ] public DateTime?                      EnabledDt    { get; set; } // datetime

		#region Associations

		/// <summary>
		/// ClientAvailableScopesToClient_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ClientId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ClientAvailableScopeDb> ClientAvailableScopesToClients { get; set; }

		/// <summary>
		/// ClientAvailableUriToClient_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ClientId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<ClientAvailableUriDb> ClientAvailableUriToClients { get; set; }

		/// <summary>
		/// LoginSessionToClient_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ClientId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<LoginSessionDb> LoginSessionToClients { get; set; }

		#endregion
	}

	/// <summary>
	/// Login sessions
	/// </summary>
	[Table("login_sessions")]
	public partial class LoginSessionDb
	{
		/// <summary>
		/// Login session identifier (GUID)
		/// </summary>
		[Column("id"),               PrimaryKey,  NotNull] public string                                                       Id             { get; set; } // char(32)
		/// <summary>
		/// Client identifier
		/// </summary>
		[Column("client_id"),                     NotNull] public string                                                       ClientId       { get; set; } // char(32)
		/// <summary>
		/// Request &apos;redirect_uri&apos;
		/// </summary>
		[Column("redirect_uri"),                  NotNull] public string                                                       RedirectUri    { get; set; } // varchar(250)
		/// <summary>
		/// Request &apos;scope&apos;
		/// </summary>
		[Column("scope"),                         NotNull] public string                                                       Scope          { get; set; } // varchar(250)
		/// <summary>
		/// Request &apos;state&apos;
		/// </summary>
		[Column("state"),               Nullable         ] public string                                                       State          { get; set; } // varchar(250)
		/// <summary>
		/// Createion date time
		/// </summary>
		[Column("create_dt"),                     NotNull] public DateTime                                                     CreateDt       { get; set; } // datetime
		/// <summary>
		/// Authorization error string code
		/// </summary>
		[Column("error_code"),          Nullable         ] public MyAuth.OAuthPoint.Models.AuthorizationRequestProcessingError ErrorCode      { get; set; } // varchar(50)
		/// <summary>
		/// Authorization error description
		/// </summary>
		[Column("error_desc"),          Nullable         ] public string                                                       ErrorDesc      { get; set; } // varchar(250)
		/// <summary>
		/// Authorized subject identifier
		/// </summary>
		[Column("subject_id"),          Nullable         ] public string                                                       SubjectId      { get; set; } // varchar(50)
		/// <summary>
		/// Session expirration date time
		/// </summary>
		[Column("expiry"),                        NotNull] public DateTime                                                     Expiry         { get; set; } // datetime
		/// <summary>
		/// Authorization code
		/// </summary>
		[Column("auth_code"),           Nullable         ] public string                                                       AuthCode       { get; set; } // char(32)
		/// <summary>
		/// Authorization code expiration date time
		/// </summary>
		[Column("auth_code_expiry"),    Nullable         ] public DateTime?                                                    AuthCodeExpiry { get; set; } // datetime
		/// <summary>
		/// Indicates that authorixation code already used
		/// </summary>
		[Column("auth_code_used"),      Nullable         ] public MyAuth.OAuthPoint.Db.MySqlBool                               AuthCodeUsed   { get; set; } // enum('Y','N')
		/// <summary>
		/// Login completion expiration date time
		/// </summary>
		[Column("login_expiry"),                  NotNull] public DateTime                                                     LoginExpiry    { get; set; } // datetime
		/// <summary>
		/// Indicates that session logon or error
		/// </summary>
		[Column("completed"),           Nullable         ] public MyAuth.OAuthPoint.Db.MySqlBool                               Completed      { get; set; } // enum('Y','N')
		/// <summary>
		/// &apos;completed&apos; field actuallity date time
		/// </summary>
		[Column("completed_dt"),        Nullable         ] public DateTime?                                                    CompletedDt    { get; set; } // datetime

		#region Associations

		/// <summary>
		/// LoginSessionToClient
		/// </summary>
		[Association(ThisKey="ClientId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="LoginSessionToClient", BackReferenceName="LoginSessionToClients")]
		public ClientDb Client { get; set; }

		/// <summary>
		/// LoginSessionToSubject
		/// </summary>
		[Association(ThisKey="SubjectId", OtherKey="Id", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="LoginSessionToSubject", BackReferenceName="LoginSessionToSubjects")]
		public SubjectDb Subject { get; set; }

		#endregion
	}

	/// <summary>
	/// Additional subject claims for access token
	/// </summary>
	[Table("subject_access_claims")]
	public partial class SubjectAccessClaimDb
	{
		/// <summary>
		/// Row id
		/// </summary>
		[Column("id"),         PrimaryKey, Identity] public int    Id        { get; set; } // int
		/// <summary>
		/// Name
		/// </summary>
		[Column("name"),       NotNull             ] public string Name      { get; set; } // varchar(50)
		/// <summary>
		/// Value
		/// </summary>
		[Column("value"),      NotNull             ] public string Value     { get; set; } // varchar(250)
		/// <summary>
		/// Subject identifier
		/// </summary>
		[Column("subject_id"), NotNull             ] public string SubjectId { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// SubjectAccessClaimsToSubject
		/// </summary>
		[Association(ThisKey="SubjectId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="SubjectAccessClaimsToSubject", BackReferenceName="SubjectAccessClaimsToSubjects")]
		public SubjectDb Subject { get; set; }

		#endregion
	}

	/// <summary>
	/// Subject available scopes
	/// </summary>
	[Table("subject_available_scopes")]
	public partial class SubjectAvailableScopeDb
	{
		/// <summary>
		/// Row identifier
		/// </summary>
		[Column("id"),         PrimaryKey, Identity] public int    Id        { get; set; } // int
		/// <summary>
		/// Scope name
		/// </summary>
		[Column("name"),       NotNull             ] public string Name      { get; set; } // varchar(50)
		/// <summary>
		/// Subject identifier
		/// </summary>
		[Column("subject_id"), NotNull             ] public string SubjectId { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// SubjectAvailabeScopesToSubject
		/// </summary>
		[Association(ThisKey="SubjectId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="SubjectAvailabeScopesToSubject", BackReferenceName="SubjectAvailabeScopesToSubjects")]
		public SubjectDb Subject { get; set; }

		#endregion
	}

	/// <summary>
	/// Subjects
	/// </summary>
	[Table("subjects")]
	public partial class SubjectDb
	{
		/// <summary>
		/// String identitifer
		/// </summary>
		[Column("id"),         PrimaryKey,  NotNull] public string                         Id        { get; set; } // varchar(50)
		/// <summary>
		/// Indicates is subject enabled
		/// </summary>
		[Column("enabled"),                 NotNull] public MyAuth.OAuthPoint.Db.MySqlBool Enabled   { get; set; } // enum('Y','N')
		/// <summary>
		/// &apos;enabled&apos; flag actiallity datetime
		/// </summary>
		[Column("enabled_dt"),    Nullable         ] public DateTime?                      EnabledDt { get; set; } // datetime

		#region Associations

		/// <summary>
		/// LoginSessionToSubject_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SubjectId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<LoginSessionDb> LoginSessionToSubjects { get; set; }

		/// <summary>
		/// SubjectAccessClaimsToSubject_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SubjectId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<SubjectAccessClaimDb> SubjectAccessClaimsToSubjects { get; set; }

		/// <summary>
		/// SubjectAvailabeScopesToSubject_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SubjectId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<SubjectAvailableScopeDb> SubjectAvailabeScopesToSubjects { get; set; }

		/// <summary>
		/// SubjectIdentityClaimsToSubject_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="SubjectId", CanBeNull=true, Relationship=LinqToDB.Mapping.Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<SubjectIdentityClaimDb> SubjectIdentityClaimsToSubjects { get; set; }

		#endregion
	}

	/// <summary>
	/// Subject claims for identity
	/// </summary>
	[Table("subject_identity_claims")]
	public partial class SubjectIdentityClaimDb
	{
		/// <summary>
		/// Row id
		/// </summary>
		[Column("id"),         PrimaryKey, Identity] public int    Id        { get; set; } // int
		/// <summary>
		/// Name
		/// </summary>
		[Column("name"),       NotNull             ] public string Name      { get; set; } // varchar(50)
		/// <summary>
		/// Value
		/// </summary>
		[Column("value"),      NotNull             ] public string Value     { get; set; } // varchar(250)
		/// <summary>
		/// Scope string identifier
		/// </summary>
		[Column("scope_id"),   NotNull             ] public string ScopeId   { get; set; } // varchar(50)
		/// <summary>
		/// Subject id
		/// </summary>
		[Column("subject_id"), NotNull             ] public string SubjectId { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// SubjectIdentityClaimsToSubject
		/// </summary>
		[Association(ThisKey="SubjectId", OtherKey="Id", CanBeNull=false, Relationship=LinqToDB.Mapping.Relationship.ManyToOne, KeyName="SubjectIdentityClaimsToSubject", BackReferenceName="SubjectIdentityClaimsToSubjects")]
		public SubjectDb Subject { get; set; }

		#endregion
	}

	public static partial class TableExtensions
	{
		public static ClientAvailableScopeDb Find(this ITable<ClientAvailableScopeDb> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ClientAvailableUriDb Find(this ITable<ClientAvailableUriDb> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ClientDb Find(this ITable<ClientDb> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static LoginSessionDb Find(this ITable<LoginSessionDb> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static SubjectAccessClaimDb Find(this ITable<SubjectAccessClaimDb> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static SubjectAvailableScopeDb Find(this ITable<SubjectAvailableScopeDb> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static SubjectDb Find(this ITable<SubjectDb> table, string Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static SubjectIdentityClaimDb Find(this ITable<SubjectIdentityClaimDb> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}

#pragma warning restore 1591
