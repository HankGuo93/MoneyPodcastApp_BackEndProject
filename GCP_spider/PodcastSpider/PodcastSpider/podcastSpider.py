import scrapy
import re
from twisted.internet import reactor
#from scrapy.crawler import CrawlerProcess
from scrapy.crawler import CrawlerRunner
from scrapy.utils.project import get_project_settings
from scrapy.utils.log import configure_logging

class PodcastspiderSpider(scrapy.Spider):
    name = 'podcastSpider'
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
                    'api.soundon.fm/v2/podcasts/112ff502-bab6-4695-a926-702a6e1eb0b2/feed.xml'
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
                  'https://api.soundon.fm/v2/podcasts/112ff502-bab6-4695-a926-702a6e1eb0b2/feed.xml'
                ]

    def parse(self, response):
        yield from self.scrape(response)
    
    def scrape(self, response):
        dataLen = len(response.xpath("//item/enclosure/@url").extract())
        response.selector.register_namespace('itunes','http://www.itunes.com/dtds/podcast-1.0.dtd')
        authors = self.createList(dataLen, response.xpath("//channel/title/text()").extract())
        titles = response.xpath("//item/title/text()").extract()
        mp3 = response.xpath("//item/enclosure/@url").extract()
        desc = response.xpath("//item/description/text()").extract()
        duration = response.xpath("//item/itunes:duration/text()").extract()
        img = self.createList(dataLen, response.xpath("//itunes:image/@href").extract())
        pubDate = response.xpath("//item/pubDate/text()").extract()
        
        for item in zip(authors,titles,mp3,desc,duration,img,pubDate):
            scraped_info = {
                'author' : self.clearTag(item[0]),
                'title' : self.clearTag(item[1]),
                'mp3' : self.clearTag(item[2]),
                'desc' : self.clearTag(item[3]),
                'duration' : self.clearTag(item[4]),
                'img' : self.clearTag(item[5]),
                'pubDate': self.clearTag(item[6])
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
'''          
configure_logging({'LOG_FORMAT': '%(levelname)s: %(message)s'})
#process = CrawlerProcess(get_project_settings())
#process.crawl(PodcasttestSpider)
print('~~~~~~~~~~~~ Processing is going to be started ~~~~~~~~~~')
#process.start() # the script will block here until the crawling is finished
runner = CrawlerRunner(get_project_settings())

d = runner.crawl(PodcasttestSpider)
d.addBoth(lambda _: reactor.stop())
reactor.run()
print('~~~~~~~~~~~~ Processing ended ~~~~~~~~~~')
#process.join()
'''
