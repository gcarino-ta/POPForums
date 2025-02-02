﻿namespace PopForums.Test.Services;

public class SubscribedTopicsServiceTests
{
	private Mock<ISubscribedTopicsRepository> _mockSubRepo;
	private Mock<ISettingsManager> _mockSettingsManager;
	private Mock<INotificationAdapter> _mockNotificationAdapter;
	private Mock<ISubscribeNotificationRepository> _subNotificationRepo;

	private SubscribedTopicsService GetService()
	{
		_mockSubRepo = new Mock<ISubscribedTopicsRepository>();
		_mockSettingsManager = new Mock<ISettingsManager>();
		_mockNotificationAdapter = new Mock<INotificationAdapter>();
		_subNotificationRepo = new Mock<ISubscribeNotificationRepository>();
		return new SubscribedTopicsService(_mockSubRepo.Object, _mockSettingsManager.Object, _mockNotificationAdapter.Object, _subNotificationRepo.Object);
	}

	[Fact]
	public async Task AddSubTopic()
	{
		var service = GetService();
		var user = new User { UserID = 123 };
		var topic = new Topic { TopicID = 456 };
		_mockSubRepo.Setup(x => x.IsTopicSubscribed(user.UserID, topic.TopicID)).ReturnsAsync(false);

		await service.AddSubscribedTopic(user.UserID, topic.TopicID);

		_mockSubRepo.Verify(s => s.AddSubscribedTopic(user.UserID, topic.TopicID), Times.Once);
	}

	[Fact]
	public async Task DoNotAddSubTopicIfAlreadySub()
	{
		var service = GetService();
		var user = new User { UserID = 123 };
		var topic = new Topic { TopicID = 456 };
		_mockSubRepo.Setup(x => x.IsTopicSubscribed(user.UserID, topic.TopicID)).ReturnsAsync(true);

		await service.AddSubscribedTopic(user.UserID, topic.TopicID);

		_mockSubRepo.Verify(s => s.AddSubscribedTopic(user.UserID, topic.TopicID), Times.Never);
	}

	[Fact]
	public async Task RemoveSubTopic()
	{
		var service = GetService();
		var user = new User { UserID = 123 };
		var topic = new Topic { TopicID = 456 };
		await service.RemoveSubscribedTopic(user, topic);
		_mockSubRepo.Verify(s => s.RemoveSubscribedTopic(user.UserID, topic.TopicID), Times.Once());
	}

	[Fact]
	public async Task TryRemoveSubTopic()
	{
		var service = GetService();
		var user = new User { UserID = 123 };
		var topic = new Topic { TopicID = 456 };
		await service.TryRemoveSubscribedTopic(user, topic);
		_mockSubRepo.Verify(s => s.RemoveSubscribedTopic(user.UserID, topic.TopicID), Times.Once());
	}

	[Fact]
	public async Task TryRemoveSubTopicNullTopic()
	{
		var service = GetService();
		var user = new User { UserID = 123 };
		await service.TryRemoveSubscribedTopic(user, null);
		_mockSubRepo.Verify(s => s.RemoveSubscribedTopic(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
	}

	[Fact]
	public async Task TryRemoveSubTopicNullUser()
	{
		var service = GetService();
		var topic = new Topic { TopicID = 456 };
		await service.TryRemoveSubscribedTopic(null, topic);
		_mockSubRepo.Verify(s => s.RemoveSubscribedTopic(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
	}
		
	[Fact]
	public async Task GetTopicsFromRepo()
	{
		var user = new User { UserID = 123 };
		var service = GetService();
		var settings = new Settings { TopicsPerPage = 20 };
		_mockSettingsManager.Setup(s => s.Current).Returns(settings);
		var list = new List<Topic>();
		_mockSubRepo.Setup(s => s.GetSubscribedTopics(user.UserID, 1, 20)).ReturnsAsync(list);
		var (result, _) = await service.GetTopics(user, 1);
		Assert.Same(list, result);
	}

	[Fact]
	public async Task GetTopicsStartRowCalcd()
	{
		var user = new User { UserID = 123 };
		var service = GetService();
		var settings = new Settings { TopicsPerPage = 20 };
		_mockSettingsManager.Setup(s => s.Current).Returns(settings);
		var (_, pagerContext) = await service.GetTopics(user, 3);
		_mockSubRepo.Verify(s => s.GetSubscribedTopics(user.UserID, 41, 20), Times.Once());
		Assert.Equal(20, pagerContext.PageSize);
	}
}