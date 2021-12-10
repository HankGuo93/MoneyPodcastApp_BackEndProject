import scrapy
import re
import os
from twisted.internet import reactor
#from scrapy.crawler import CrawlerProcess
from scrapy.crawler import CrawlerRunner
from scrapy.utils.project import get_project_settings
from scrapy.utils.log import configure_logging


class PodcastspiderSpider(scrapy.Spider):
    if os.path.isfile('daliyPodcast.json'):
        os.remove('daliyPodcast.json')
    name = 'podcastDailySpider'
    allowed_domains = [
                    'api.soundon.fm/v2/podcasts/954689a5-3096-43a4-a80b-7810b219cef3/feed.xml',
                    'api.soundon.fm/v2/podcasts/b8f5a471-f4f7-4763-9678-65887beda63a/feed.xml',
                    'api.soundon.fm/v2/podcasts/f1a1dd8f-7aa4-44cf-92e6-c687f0459091/feed.xml',
                    'api.soundon.fm/v2/podcasts/05e91c72-16bf-4f6a-8632-4d52c0d0605b/feed.xml',
                    'api.soundon.fm/v2/podcasts/d2aab16c-3a70-4023-b52b-e50f07852ecd/feed.xml',
                    'feeds.soundcloud.com/users/soundcloud:users:735679489/sounds.rss',
                    'api.soundon.fm/v2/podcasts/c92cb66a-14fc-412f-84aa-08125add5ca9/feed.xml',
                    'anchor.fm/s/120ba8d0/podcast/rss',
                    'www.himalaya.com/rss/2261219.xml',
                    'feeds.buzzsprout.com/812006.rss',
                    'feed.podbean.com/meigutalkshow/feed.xml',
                    'open.firstory.me/rss/user/ckgeyi4eh7hea0875ttj3gtga',
                    'api.soundon.fm/v2/podcasts/112ff502-bab6-4695-a926-702a6e1eb0b2/feed.xml',
                    'api.soundon.fm/v2/podcasts/497fc565-90fa-48de-90cd-e984bfa4e8c2/feed.xml'
                    ]

    start_urls = [
                  'https://api.soundon.fm/v2/podcasts/954689a5-3096-43a4-a80b-7810b219cef3/feed.xml',
                  'https://api.soundon.fm/v2/podcasts/b8f5a471-f4f7-4763-9678-65887beda63a/feed.xml',
                  'https://api.soundon.fm/v2/podcasts/f1a1dd8f-7aa4-44cf-92e6-c687f0459091/feed.xml',
                  'https://api.soundon.fm/v2/podcasts/05e91c72-16bf-4f6a-8632-4d52c0d0605b/feed.xml',
                  'https://api.soundon.fm/v2/podcasts/d2aab16c-3a70-4023-b52b-e50f07852ecd/feed.xml',
                  'http://feeds.soundcloud.com/users/soundcloud:users:735679489/sounds.rss',
                  'https://api.soundon.fm/v2/podcasts/c92cb66a-14fc-412f-84aa-08125add5ca9/feed.xml',
                  'https://anchor.fm/s/120ba8d0/podcast/rss',
                  'https://www.himalaya.com/rss/2261219.xml',
                  'https://feeds.buzzsprout.com/812006.rss',
                  'https://feed.podbean.com/meigutalkshow/feed.xml',
                  'https://open.firstory.me/rss/user/ckgeyi4eh7hea0875ttj3gtga',
                  'https://api.soundon.fm/v2/podcasts/112ff502-bab6-4695-a926-702a6e1eb0b2/feed.xml',
                  'https://api.soundon.fm/v2/podcasts/497fc565-90fa-48de-90cd-e984bfa4e8c2/feed.xml'
                ]
    '''
    def parse(self, response):
        yield from self.scrape(response)
    '''
    def parse(self, response):
        response.selector.register_namespace('itunes','http://www.itunes.com/dtds/podcast-1.0.dtd')
        authors = [self.clearTag(response.xpath("//channel/title/text()").get(0))]
        titles = [self.clearTag(response.xpath("//item/title/text()").get(0))]
        mp3 = [self.clearTag(response.xpath("//item/enclosure/@url").get(0))]
        desc = [self.clearTag(response.xpath("//item/description/text()").get(0))]
        duration = [self.clearTag(response.xpath("//item/itunes:duration/text()").get(0))]
        img = [self.clearTag(response.xpath("//itunes:image/@href").get(0))]
        pubDate = [self.clearTag(response.xpath("//item/pubDate/text()").get(0))]

        for item in zip(authors,titles,mp3,desc,duration,img,pubDate):
            scraped_info = {
                'author' : item[0],
                'title' : item[1],
                'mp3' : item[2],
                'desc' : item[3],
                'duration' : item[4],
                'img' : item[5],
                'pubDate': item[6]
            }
            yield scraped_info

    def createList(self, times, items):
        i = 1
        while i < times:
            items.append(items[0])
            i += 1
        return items

    def clearTag(self, item):
        pattern = re.compile(r'<[^>]+>', re.S)
        return pattern.sub(' ', item)

os.system("scrapy crawl podcastDailySpider --output=daliyPodcast.json")
