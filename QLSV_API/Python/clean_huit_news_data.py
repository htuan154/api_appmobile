import json
import os
import re
import csv
from datetime import datetime

# Đường dẫn đến thư mục Python nơi script đang chạy
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
# Đường dẫn đầy đủ đến file JSON gốc
INPUT_JSON_FILE_PATH = os.path.join(SCRIPT_DIR, 'huit_news_data.json')
# Đường dẫn đầy đủ đến file CSV đã làm sạch
OUTPUT_CSV_FILE_PATH = os.path.join(SCRIPT_DIR, 'huit_news_data_cleaned.csv')

def load_json_data():
    """Load existing JSON data"""
    try:
        if os.path.exists(INPUT_JSON_FILE_PATH):
            with open(INPUT_JSON_FILE_PATH, 'r', encoding='utf-8') as f:
                return json.load(f)
        else:
            print(f"Error: Input file {INPUT_JSON_FILE_PATH} not found!")
            return []
    except Exception as e:
        print(f"Error loading existing data: {e}")
        return []

def clean_and_format_data(data):
    """Clean and format the data to match the SQL table structure"""
    cleaned_data = []
    
    for idx, item in enumerate(data, 1):
        # Create a new record with required fields
        clean_record = {
            "Ma_TT": idx,  # Reassign ID to ensure sequential numbering
            "Ma_TK": "TK0001",  # Add the Ma_TK field with value A0001
            "NoiDung": item.get("NoiDung", "").strip(),  # Get NoiDung and strip whitespace
            "NgayTao": format_date(item.get("NgayTao", ""))  # Format date properly
        }
        
        # Only add records with content
        if clean_record["NoiDung"]:
            cleaned_data.append(clean_record)
    
    return cleaned_data

def format_date(date_str):
    """Format date to ensure consistency"""
    if not date_str:
        return datetime.now().strftime("%d/%m/%Y")
    
    # Try to parse common date formats
    try:
        # If the date is already in dd/mm/yyyy format
        if re.match(r'^\d{1,2}/\d{1,2}/\d{4}$', date_str):
            return date_str
            
        # If the date is in dd-mm-yyyy format
        elif re.match(r'^\d{1,2}-\d{1,2}-\d{4}$', date_str):
            day, month, year = date_str.split('-')
            return f"{day}/{month}/{year}"
            
        # If the date is in yyyy/mm/dd format
        elif re.match(r'^\d{4}/\d{1,2}/\d{1,2}$', date_str):
            year, month, day = date_str.split('/')
            return f"{day}/{month}/{year}"
            
        # If the date is in yyyy-mm-dd format  
        elif re.match(r'^\d{4}-\d{1,2}-\d{1,2}$', date_str):
            year, month, day = date_str.split('-')
            return f"{day}/{month}/{year}"
        
        # Add more format handling as needed
        
        # If no format recognized, return as is
        return date_str
        
    except Exception:
        # If date parsing fails, return today's date
        return datetime.now().strftime("%d/%m/%Y")

def export_to_csv(cleaned_data):
    """Export cleaned data to CSV file"""
    try:
        # Define CSV headers
        fieldnames = ['Ma_TT', 'Ma_TK', 'NoiDung', 'NgayTao']
        
        # Write data to CSV file
        with open(OUTPUT_CSV_FILE_PATH, 'w', encoding='utf-8', newline='') as csvfile:
            writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
            
            # Write header
            writer.writeheader()
            
            # Write data rows
            for item in cleaned_data:
                writer.writerow(item)
                
        print(f"Successfully exported {len(cleaned_data)} records to {OUTPUT_CSV_FILE_PATH}")
    except Exception as e:
        print(f"Error exporting to CSV: {e}")

if __name__ == "__main__":
    print(f"Starting data cleaning process...")
    
    # Load the existing JSON data
    print(f"Loading data from {INPUT_JSON_FILE_PATH}")
    data = load_json_data()
    
    if data:
        print(f"Loaded {len(data)} records.")
        
        # Clean and format the data
        print("Cleaning and formatting data...")
        cleaned_data = clean_and_format_data(data)
        
        # Export to CSV format
        print("Exporting data to CSV...")
        export_to_csv(cleaned_data)
        
        # Display sample of cleaned data
        print("\nSample of cleaned data:")
        sample_size = min(5, len(cleaned_data))
        for item in cleaned_data[:sample_size]:
            print(f"Ma_TT: {item['Ma_TT']}")
            print(f"Ma_TK: {item['Ma_TK']}")
            print(f"NoiDung: {item['NoiDung']}")
            print(f"NgayTao: {item['NgayTao']}")
            print("-" * 50)
    else:
        print("No data to process. Please check your input file.")