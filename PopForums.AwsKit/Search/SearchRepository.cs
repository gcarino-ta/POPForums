﻿using System;
using System.Collections.Generic;
using PopForums.Configuration;
using PopForums.Models;
using PopForums.Repositories;
using PopForums.Sql;

namespace PopForums.AwsKit.Search
{
	public class SearchRepository : Sql.Repositories.SearchRepository
	{
		private readonly IConfig _config;
		private readonly IErrorLog _errorLog;
		private readonly ITopicRepository _topicRepository;
		private readonly IElasticSearchClientWrapper _elasticSearchClientWrapper;

		public SearchRepository(ISqlObjectFactory sqlObjectFactory, IConfig config, IErrorLog errorLog,
			ITopicRepository topicRepository, IElasticSearchClientWrapper elasticSearchClientWrapper) : base(sqlObjectFactory)
		{
			_config = config;
			_errorLog = errorLog;
			_topicRepository = topicRepository;
			_elasticSearchClientWrapper = elasticSearchClientWrapper;
		}

		public override List<string> GetJunkWords()
		{
			return new List<string>();
		}

		public override void CreateJunkWord(string word)
		{
			throw new NotImplementedException();
		}

		public override void DeleteJunkWord(string word)
		{
			throw new NotImplementedException();
		}

		// GetNextTopicForIndexing() uses base

		// MarkTopicAsIndexed(int topicID) uses base

		public override void DeleteAllIndexedWordsForTopic(int topicID)
		{
			throw new NotImplementedException();
		}

		public override void SaveSearchWord(int topicID, string word, int rank)
		{
			throw new NotImplementedException();
		}

		public override List<Topic> SearchTopics(string searchTerm, List<int> hiddenForums, SearchType searchType, int startRow, int pageSize, out int topicCount)
		{
			var ids = _elasticSearchClientWrapper.SearchTopicsWithIDs(searchTerm, hiddenForums, searchType, startRow, pageSize, out topicCount);
			var topics = _topicRepository.Get(ids);
			return topics;
		}
	}
}