using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Linq;

namespace JohnKnoop.MongoRepository
{
	public class SoftDeletedEntity<TEntity>
	{
		internal SoftDeletedEntity(TEntity entity, DateTime timestampDeletedUtc)
		{
			Entity = entity;
			TimestampDeletedUtc = timestampDeletedUtc;
		}

		public TEntity Entity { get; private set; }
		public DateTime TimestampDeletedUtc { get; private set; }
	}

	public class SoftDeletedEntity
	{
		public SoftDeletedEntity(string typeName, string sourceCollectionName, DateTime timestampDeletedUtc)
		{
			TypeName = typeName;
			SourceCollectionName = sourceCollectionName;
			TimestampDeletedUtc = timestampDeletedUtc;
		}

		public string TypeName { get; private set; }
		public string SourceCollectionName { get; private set; }
		public DateTime TimestampDeletedUtc { get; private set; }
	}

	public interface IRepository<TEntity>
	{
		Task InsertAsync(TEntity entity);
		Task InsertManyAsync(ICollection<TEntity> entities);
		Task InsertManyAsync<TDerivedEntity>(ICollection<TDerivedEntity> entities) where TDerivedEntity : TEntity;

		IMongoQueryable<TEntity> Query();
	    IMongoQueryable<TDerivedEntity> Query<TDerivedEntity>() where TDerivedEntity : TEntity;
		
		/// <returns>An instance of <c>TEntity</c> or null</returns>
		Task<TEntity> GetAsync(string id);
		/// <returns>An instance of <c>T</c> or null</returns>
		Task<TDerivedEntity> GetAsync<TDerivedEntity>(string id) where TDerivedEntity : TEntity;
		
		Task<TReturnProjection> GetAsync<TReturnProjection>(string id, Expression<Func<TEntity, TReturnProjection>> returnProjection);
		Task<TReturnProjection> GetAsync<TDerivedEntity, TReturnProjection>(string id, Expression<Func<TDerivedEntity, TReturnProjection>> returnProjection) where TDerivedEntity : TEntity;

		IFindFluent<TEntity, TEntity> GetAll();
		IFindFluent<TEntity, TEntity> Find(Expression<Func<TEntity, bool>> filter);
		IFindFluent<TDerivedEntity, TDerivedEntity> Find<TDerivedEntity>(Expression<Func<TDerivedEntity, bool>> filter) where TDerivedEntity : TEntity;
		
		IFindFluent<TEntity, TEntity> Find(Expression<Func<TEntity, object>> property, string regexPattern, string regexOptions = "i");
		IFindFluent<TEntity, TEntity> Find(FieldDefinition<TEntity> property, string regexPattern, string regexOptions = "i");
		IFindFluent<TEntity, TEntity> Find(IEnumerable<Expression<Func<TEntity, object>>> properties, string regexPattern, string regexOptions = "i");
		IFindFluent<TEntity, TEntity> Find(IEnumerable<FieldDefinition<TEntity>> properties, string regexPattern, string regexOptions = "i");

		IFindFluent<TDerivedEntity, TDerivedEntity> Find<TDerivedEntity>(Expression<Func<TDerivedEntity, object>> property, string regexPattern, string regexOptions = "i") where TDerivedEntity : TEntity;
		IFindFluent<TDerivedEntity, TDerivedEntity> Find<TDerivedEntity>(FieldDefinition<TDerivedEntity> property, string regexPattern, string regexOptions = "i") where TDerivedEntity : TEntity;
		IFindFluent<TDerivedEntity, TDerivedEntity> Find<TDerivedEntity>(IEnumerable<Expression<Func<TDerivedEntity, object>>> properties, string regexPattern, string regexOptions = "i") where TDerivedEntity : TEntity;
		IFindFluent<TDerivedEntity, TDerivedEntity> Find<TDerivedEntity>(IEnumerable<FieldDefinition<TDerivedEntity>> properties, string regexPattern, string regexOptions = "i") where TDerivedEntity : TEntity;

		Task<IAsyncCursor<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter);
		Task<IAsyncCursor<TDerivedEntity>> FindAsync<TDerivedEntity>(Expression<Func<TDerivedEntity, bool>> filter) where TDerivedEntity : TEntity;
		Task<IAsyncCursor<TReturnProjection>> FindAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TReturnProjection>> returnProjection);
		Task<IAsyncCursor<TReturnProjection>> FindAsync<TDerivedEntity, TReturnProjection>(Expression<Func<TDerivedEntity, bool>> filter, Expression<Func<TDerivedEntity, TReturnProjection>> returnProjection) where TDerivedEntity : TEntity;

		Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filter);
		Task<TDerivedEntity> FindOneAsync<TDerivedEntity>(Expression<Func<TDerivedEntity, bool>> filter) where TDerivedEntity : TEntity;
		Task<TReturnProjection> FindOneAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TReturnProjection>> returnProjection);
		Task<TReturnProjection> FindOneAsync<TDerivedEntity, TReturnProjection>(Expression<Func<TDerivedEntity, bool>> filter, Expression<Func<TDerivedEntity, TReturnProjection>> returnProjection) where TDerivedEntity : TEntity;

		IFindFluent<TEntity, TEntity> Find(FilterDefinition<TEntity> filter);

		IFindFluent<TDerivedEntity, TDerivedEntity> Find<TDerivedEntity>(FilterDefinition<TDerivedEntity> filter)
			where TDerivedEntity : TEntity;

		IAggregateFluent<TEntity> Aggregate(AggregateOptions options = null);
	    Task DeletePropertyAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> propertyExpression);

        Task<DeleteResult> DeleteByIdAsync(string id, bool softDelete = false);
		Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, bool softDelete = false);
		Task<DeleteResult> DeleteManyAsync<TDerived>(Expression<Func<TDerived, bool>> filter, bool softDelete = false) where TDerived : TEntity;

		Task<TEntity> GetFromTrashAsync(Expression<Func<SoftDeletedEntity<TEntity>, bool>> filter);
		Task<IList<TEntity>> RestoreSoftDeletedAsync(Expression<Func<SoftDeletedEntity<TEntity>, bool>> filter);
		Task<IList<SoftDeletedEntity>> ListTrashAsync(int? offset = null, int? limit = null);

		Task<TEntity> RestoreSoftDeletedAsync(string objectId);
		Task<IList<TDerived>> RestoreSoftDeletedAsync<TDerived>(Expression<Func<SoftDeletedEntity<TDerived>, bool>> filter) where TDerived : TEntity;
		
		Task<long> PermamentlyDeleteSoftDeletedAsync(Expression<Func<SoftDeletedEntity<TEntity>, bool>> filter);

		Task<ReplaceOneResult> ReplaceOneAsync(string id, TEntity entity, bool upsert = false);
		Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity, bool upsert = false);
		Task<BulkWriteResult<TEntity>> ReplaceManyAsync(IList<ReplaceManyCommand<TEntity>> commands, bool upsert = false);

        [Obsolete("This overload of UpdateOneAsync will be removed in a future version")]
        Task<UpdateResult> UpdateOneAsync(string filter, string update, bool upsert = false);

		Task<UpdateResult> UpdateOneAsync(string id, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, bool upsert = false);
		Task<UpdateResult> UpdateOneAsync(string id, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, UpdateOptions options);


        Task<UpdateResult> UpdateOneAsync(Expression<Func<TEntity, bool>> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, bool upsert = false);
        Task<UpdateResult> UpdateOneAsync(FilterDefinition<TEntity> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, bool upsert = false);
        
        Task<UpdateResult> UpdateOneAsync(Expression<Func<TEntity, bool>> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, UpdateOptions options);
        Task<UpdateResult> UpdateOneAsync(FilterDefinition<TEntity> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, UpdateOptions options);
		
        Task<UpdateResult> UpdateOneAsync<TDerived>(string id, Func<UpdateDefinitionBuilder<TDerived>, UpdateDefinition<TDerived>> update, bool upsert = false) where TDerived : TEntity;
        Task<UpdateResult> UpdateOneAsync<TDerived>(string id, Func<UpdateDefinitionBuilder<TDerived>, UpdateDefinition<TDerived>> update, UpdateOptions options) where TDerived : TEntity;
		
        Task<UpdateResult> UpdateOneAsync<TDerived>(Expression<Func<TDerived, bool>> filter, Func<UpdateDefinitionBuilder<TDerived>, UpdateDefinition<TDerived>> update, bool upsert = false) where TDerived : TEntity;
        Task<UpdateResult> UpdateOneAsync<TDerived>(FilterDefinition<TDerived> filter, Func<UpdateDefinitionBuilder<TDerived>, UpdateDefinition<TDerived>> update, bool upsert = false) where TDerived : TEntity;
        Task<UpdateResult> UpdateOneAsync<TDerived>(Expression<Func<TDerived, bool>> filter, Func<UpdateDefinitionBuilder<TDerived>, UpdateDefinition<TDerived>> update, UpdateOptions options) where TDerived : TEntity;
        Task<UpdateResult> UpdateOneAsync<TDerived>(FilterDefinition<TDerived> filter, Func<UpdateDefinitionBuilder<TDerived>, UpdateDefinition<TDerived>> update, UpdateOptions options) where TDerived : TEntity;


		Task<TReturnProjection> FindOneAndUpdateAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, Expression<Func<TEntity, TReturnProjection>> returnProjection, ReturnedDocumentState returnedDocumentState = ReturnedDocumentState.AfterUpdate, bool upsert = false);
		Task<TReturnProjection> FindOneAndUpdateAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, ProjectionDefinition<TEntity, TReturnProjection> returnProjection, ReturnedDocumentState returnedDocumentState = ReturnedDocumentState.AfterUpdate, bool upsert = false);

		/// <summary>
		/// Executes multiple update operations in one batch
		/// </summary>
		Task<BulkWriteResult<TEntity>> UpdateOneBulkAsync(IEnumerable<UpdateOneCommand<TEntity>> commands);

		/// <summary>
		/// Executes multiple update operations in one batch
		/// </summary>
		Task<BulkWriteResult<TDerived>> UpdateOneBulkAsync<TDerived>(IEnumerable<UpdateOneCommand<TDerived>> commands) where TDerived : TEntity;

		/// <summary>
		/// Applies the same update to multiple entities
		/// </summary>
		Task UpdateManyAsync(Expression<Func<TEntity, bool>> filter,
			Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update,
			UpdateOptions options = null);

        [Obsolete("This overload of UpdateOneAsync will be removed in a future version")]
		Task UpdateManyAsync(Expression<Func<TEntity, bool>> filter,
			string update,
			UpdateOptions options = null);

		/// <summary>
		/// Increments a counter (stored in the _counters collection) and returns the new value
		/// </summary>
		/// <param name="name">The name of the counter. Use if you need more than one counter per collection.</param>
		/// <param name="incrementBy">The size of the increment</param>
		/// <returns>The new value of the counter</returns>
		Task<long> IncrementCounterAsync(string name = null, int incrementBy = 1);
		Task<long?> GetCounterValueAsync(string name = null);
		Task ResetCounterAsync(string name = null, long newValue = 1);
		
		/// <summary>
		/// Updates the value of a counter if the new value is greater than the current one. If the counter doesn't exist, it will be created.
		/// </summary>
		/// <returns>The value of the counter after the operation is done</returns>
		Task<long> SetCounterValueIfGreaterAsync(long newValue, string name = null);
		
		Task<IFindFluent<TEntity, TEntity>> TextSearch(string text);
		Task<IFindFluent<TDerivedEntity, TDerivedEntity>> TextSearch<TDerivedEntity>(string text) where TDerivedEntity : TEntity;
		
		Task<TReturnProjection> FindOneAndReplaceAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter,  TEntity replacement, Expression<Func<TEntity, TReturnProjection>> returnProjection, ReturnedDocumentState returnedDocumentState = ReturnedDocumentState.AfterUpdate, bool upsert = false);
		Task<TReturnProjection> FindOneAndReplaceAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter,  TEntity replacement, ProjectionDefinition<TEntity, TReturnProjection> returnProjection, ReturnedDocumentState returnedDocumentState = ReturnedDocumentState.AfterUpdate, bool upsert = false);
		
		Task<TReturnProjection> FindOneOrInsertAsync<TReturnProjection>(Expression<Func<TEntity, bool>> filter, TEntity entity, Expression<Func<TEntity, TReturnProjection>> returnProjection, ReturnedDocumentState returnedDocumentState = ReturnedDocumentState.AfterUpdate);
		
		/// <summary>
		/// Enlists with the ambient TransactionScope, and fails or succeeds as it fails or succeeds.
		/// This is useful to be able to put a transactional boundary around MongoDB operations
		/// and things that are compatible with TransactionScopes.
		/// 
		/// This method will throw if there is no ambient TransactionScope.
		/// </summary>
		void EnlistWithCurrentTransactionScope();
		Transaction StartTransaction(ClientSessionOptions sessionOptions = null, MongoDB.Driver.TransactionOptions transactionOptions = null);
		IRepository<TEntity> WithReadPreference(ReadPreference readPreference);

		/// <summary>
		/// Starts a new transaction and executes the provided delegate.
		/// Will retry on TransientTransactionError.
		/// </summary>
		/// <param name="maxRetries">If 0, it will retry forever.</param>
		Task WithTransactionAsync(Func<Task> transactionBody, TransactionType type = TransactionType.MongoDB, int maxRetries = default);
	}

	public enum TransactionType
	{
		MongoDB,
		TransactionScope
	}

	public class TransactionEnlistment : IEnlistmentNotification
	{
		private readonly IClientSessionHandle _session;

		public TransactionEnlistment(IClientSessionHandle session)
		{
			_session = session;
		}

		public void Commit(Enlistment enlistment)
		{
			Retryer.Retry(() => _session.CommitTransaction());
			enlistment.Done();
		}

		public void InDoubt(Enlistment enlistment)
		{
			enlistment.Done();
		}

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		public void Rollback(Enlistment enlistment)
		{
			_session.AbortTransaction();
			enlistment.Done();
		}
	}

	public class Transaction : IDisposable
	{
		private readonly IClientSessionHandle _session;
		private readonly OnTransactionCompleted _onCompleted;

		private bool _isCompleted = false;

		public IClientSessionHandle Session => _session;

		internal delegate void OnTransactionCompleted(bool committed);

		internal Transaction(IClientSessionHandle session, OnTransactionCompleted onCompleted)
		{
			_session = session;
			_onCompleted = onCompleted;
		}

		public Task RetryAsync(Func<Task> transactionBody, int maxRetries = default) =>
			Retryer.RetryAsync(transactionBody, maxRetries);

		public async Task CommitAsync(CancellationToken cancellation = default)
		{
			await Retryer.RetryAsync(async() => await _session.CommitTransactionAsync(cancellation));
			_onCompleted(true);
			_isCompleted = true;
		}

		public async Task AbortAsync(CancellationToken cancellation = default)
		{
			await _session.AbortTransactionAsync(cancellation);
			_onCompleted(false);
			_isCompleted = true;
		}

		public void Dispose()
		{
			_session.Dispose();

			if (!_isCompleted)
			{
				_onCompleted(false); 
			}
		}
	}

	internal static class Retryer
	{
		private static readonly TimeSpan TransactionTimeout = TimeSpan.FromSeconds(120);

		public static async Task RetryAsync(Func<Task> transactionBody, int maxRetries = default)
		{
			var tries = 0;
			var startTime = DateTime.UtcNow;

			while (true)
			{
				try
				{
					await transactionBody();
					return;
				}
				catch (MongoException ex) when (ex.HasErrorLabel("TransientTransactionError"))
				{
					if (maxRetries != default && tries >= maxRetries &&	DateTime.UtcNow - startTime < TransactionTimeout)
					{
						throw;
					}

					tries++;
				}
			}
		}

		public static async Task Retry(Action transactionBody, int maxRetries = default)
		{
			var tries = 0;
			var startTime = DateTime.UtcNow;

			while (true)
			{
				try
				{
					transactionBody();
					return;
				}
				catch (MongoException ex) when (ex.HasErrorLabel("TransientTransactionError"))
				{
					if (maxRetries != default && tries >= maxRetries &&	DateTime.UtcNow - startTime < TransactionTimeout)
					{
						throw;
					}

					tries++;
				}
			}
		}
	}

	public class UpdateOneCommand<TEntity>
	{
		public UpdateOneCommand() { }

		public UpdateOneCommand(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update)
		{
			Filter = filter;
			Update = update;
		}

		public UpdateOneCommand(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> update, bool upsert)
			: this(filter, update)
		{
			IsUpsert = upsert;
		}

		public Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> Filter { get; set; }
		public Func<UpdateDefinitionBuilder<TEntity>, UpdateDefinition<TEntity>> Update { get; set; }
		public string UpdateJson { get; set; }
		public bool IsUpsert { get; set; }
	}

	public class ReplaceManyCommand<TEntity>
	{
		public ReplaceManyCommand() { }

		public ReplaceManyCommand(Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> filter, TEntity replacement)
		{
			Filter = filter;
			Replacement = replacement;
		}

		public Func<FilterDefinitionBuilder<TEntity>, FilterDefinition<TEntity>> Filter { get; set; }
		public TEntity Replacement { get; set; }
	}
}