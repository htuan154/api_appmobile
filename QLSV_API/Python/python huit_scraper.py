import requests
from bs4 import BeautifulSoup
import json
import re
from datetime import datetime
import os

# Đường dẫn đến thư mục Python nơi script đang chạy
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
# Đường dẫn đầy đủ đến file JSON
JSON_FILE_PATH = os.path.join(SCRIPT_DIR, 'huit_news_data.json')

def scrape_huit_news_page(url, headers, current_id=1):
    """Scrape a single page of HUIT news"""
    try:
        response = requests.get(url, headers=headers)
        response.raise_for_status()  # Raise exception for 4XX/5XX responses
        
        if response.status_code == 200:
            soup = BeautifulSoup(response.text, 'html.parser')
            
            # Find all news items on the page
            news_items = []
            
            # Try to find the news container
            news_container = soup.find('div', class_='col-md-9') or soup.find('div', class_='main-content')
            
            if news_container:
                # Look for news items - common structures in educational websites
                articles = news_container.find_all('div', class_=re.compile('item|news-item|article'))
                
                if not articles:
                    # Try alternate structures
                    articles = news_container.find_all(['div', 'article', 'li'], class_=re.compile('news|post|article|item'))
                
                if not articles:
                    # If still no results, look for table rows
                    articles = news_container.find_all('tr')
                
                # If we still don't have articles, try to get them from links
                if not articles:
                    articles = news_container.find_all('a', href=True)
                
                # Process each article/news item
                for idx, item in enumerate(articles, current_id):
                    try:
                        # Try various patterns to find the title and content
                        title_tag = item.find(['h2', 'h3', 'h4', 'a', 'strong']) or item
                        title = title_tag.get_text(strip=True)
                        
                        # Find date using common patterns
                        date_tag = item.find(['span', 'div', 'p'], class_=re.compile('date|time|created|ngay|calendar'))
                        date_text = ""
                        
                        if date_tag:
                            date_text = date_tag.get_text(strip=True)
                        else:
                            # Try to find date using regex pattern on all text
                            all_text = item.get_text(strip=True)
                            date_match = re.search(r'(\d{1,2}[/-]\d{1,2}[/-]\d{2,4}|\d{2,4}[/-]\d{1,2}[/-]\d{1,2})', all_text)
                            if date_match:
                                date_text = date_match.group(0)
                        
                        # Normalize date format if possible
                        formatted_date = date_text
                        try:
                            # Try to parse the date from various formats
                            if re.match(r'\d{1,2}[/-]\d{1,2}[/-]\d{2,4}', date_text):
                                day, month, year = re.split(r'[/-]', date_text)
                                if len(year) == 2:
                                    year = "20" + year
                                formatted_date = f"{day}/{month}/{year}"
                            elif re.match(r'\d{2,4}[/-]\d{1,2}[/-]\d{1,2}', date_text):
                                year, month, day = re.split(r'[/-]', date_text)
                                formatted_date = f"{day}/{month}/{year}"
                        except:
                            # If date parsing fails, keep original
                            pass
                            
                        news_items.append({
                            "Ma_TT": idx,
                            "NoiDung": title,
                            "NgayTao": formatted_date
                        })
                    except Exception as e:
                        print(f"Error processing item: {e}")
                        continue
                        
            # If we found no items through structured parsing, try a more generic approach
            if not news_items:
                links = soup.find_all('a', href=True)
                for idx, link in enumerate(links, current_id):
                    if link.get_text(strip=True) and len(link.get_text(strip=True)) > 10:
                        news_items.append({
                            "Ma_TT": idx,
                            "NoiDung": link.get_text(strip=True),
                            "NgayTao": datetime.now().strftime("%d/%m/%Y")  # Use current date as fallback
                        })
            
            return news_items
            
        else:
            print(f"Failed to retrieve the page. Status code: {response.status_code}")
            return []
            
    except Exception as e:
        print(f"An error occurred while scraping page {url}: {e}")
        return []


def load_existing_data():
    """Load existing JSON data if available"""
    try:
        if os.path.exists(JSON_FILE_PATH):
            with open(JSON_FILE_PATH, 'r', encoding='utf-8') as f:
                return json.load(f)
        return []
    except Exception as e:
        print(f"Error loading existing data: {e}")
        return []


def save_data(news_items):
    """Save data to JSON file"""
    try:
        # Remove duplicates based on NoiDung (content)
        unique_items = []
        seen_content = set()
        
        for item in news_items:
            if item['NoiDung'] not in seen_content:
                seen_content.add(item['NoiDung'])
                unique_items.append(item)
        
        # Reassign Ma_TT to ensure sequential numbering
        for idx, item in enumerate(unique_items, 1):
            item['Ma_TT'] = idx
        
        # Đảm bảo thư mục tồn tại
        os.makedirs(os.path.dirname(JSON_FILE_PATH), exist_ok=True)
        
        # Lưu file vào thư mục Python
        with open(JSON_FILE_PATH, 'w', encoding='utf-8') as f:
            json.dump(unique_items, f, ensure_ascii=False, indent=4)
            
        print(f"Successfully saved {len(unique_items)} unique news items to {JSON_FILE_PATH}")
    except Exception as e:
        print(f"Error saving data: {e}")


def scrape_huit_news():
    """Scrape all news from HUIT website (10 pages)"""
    base_url = "https://huit.edu.vn/dam-bao-chat-luong/tin-tuc-thong-bao.html"
    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
    }
    
    # Load existing data first
    existing_data = load_existing_data()
    all_news_items = existing_data.copy()
    
    # Scrape the first page
    print(f"Scraping page 1: {base_url}")
    page_items = scrape_huit_news_page(base_url, headers, current_id=1)
    all_news_items.extend(page_items)
    
    # Scrape pages 2-10
    for page_num in range(2, 11):
        page_url = f"{base_url}?page={page_num}"
        print(f"Scraping page {page_num}: {page_url}")
        current_id = len(all_news_items) + 1
        page_items = scrape_huit_news_page(page_url, headers, current_id=current_id)
        all_news_items.extend(page_items)
        
        # Add a small delay to be respectful to the server
        import time
        time.sleep(1)
    
    # Save all data
    save_data(all_news_items)
    return all_news_items


if __name__ == "__main__":
    print(f"Starting HUIT news scraper...")
    print(f"Script directory: {SCRIPT_DIR}")
    print(f"JSON will be saved to: {JSON_FILE_PATH}")
    news_data = scrape_huit_news()
    
    # Display the first few items
    print("\nSample of scraped data:")
    sample_size = min(5, len(news_data))
    for item in news_data[:sample_size]:
        print(f"Ma_TT: {item['Ma_TT']}")
        print(f"NoiDung: {item['NoiDung']}")
        print(f"NgayTao: {item['NgayTao']}")
        print("-" * 50)