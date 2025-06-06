import os
import csv
import pyodbc
from datetime import datetime
import re

# Đường dẫn đến file CSV
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
CSV_FILE_PATH = os.path.join(SCRIPT_DIR, 'huit_news_data_cleaned.csv')

# Cấu hình kết nối SQL Server (sử dụng Windows Authentication)
conn_str = (
    'DRIVER={ODBC Driver 17 for SQL Server};'
        'SERVER=LAPTOP-KPJ8F7HB\\SQLSERVERTHCSDL;'
        'DATABASE=QL_SV_1305;'
        'Trusted_Connection=yes;'
        'TrustServerCertificate=yes;'
)

def format_date(date_str):
    """Chuyển ngày thành dạng YYYY-MM-DD để SQL Server chấp nhận"""
    if not date_str:
        return datetime.now().strftime("%Y-%m-%d")
    try:
        if re.match(r'^\d{1,2}/\d{1,2}/\d{4}$', date_str):
            day, month, year = map(int, date_str.split('/'))
            return datetime(year, month, day).strftime("%Y-%m-%d")
        elif re.match(r'^\d{1,2}-\d{1,2}-\d{4}$', date_str):
            day, month, year = map(int, date_str.split('-'))
            return datetime(year, month, day).strftime("%Y-%m-%d")
        elif re.match(r'^\d{4}/\d{1,2}/\d{1,2}$', date_str):
            year, month, day = map(int, date_str.split('/'))
            return datetime(year, month, day).strftime("%Y-%m-%d")
        elif re.match(r'^\d{4}-\d{1,2}-\d{1,2}$', date_str):
            return date_str
        return datetime.now().strftime("%Y-%m-%d")
    except Exception:
        return datetime.now().strftime("%Y-%m-%d")

def import_csv_to_sql():
    try:
        # Kết nối tới SQL Server
        conn = pyodbc.connect(conn_str)
        cursor = conn.cursor()

        # Mở file CSV
        with open(CSV_FILE_PATH, 'r', encoding='utf-8') as csvfile:
            reader = csv.DictReader(csvfile)
            inserted = 0

            for row in reader:
                ma_tt = int(row['Ma_TT'])
                ma_tk = row['Ma_TK']
                noi_dung = row['NoiDung']
                ngay_tao = format_date(row['NgayTao'])

                # Câu lệnh INSERT
                insert_query = """
                    INSERT INTO TinTucs (Ma_TT, Ma_TK, NoiDung, NgayTao)
                    VALUES (?, ?, ?, ?)
                """
                cursor.execute(insert_query, (ma_tt, ma_tk, noi_dung, ngay_tao))
                inserted += 1

            conn.commit()
            print(f"✅ Đã chèn {inserted} dòng vào bảng TinTucs thành công.")

    except Exception as e:
        print(f"❌ Lỗi khi chèn dữ liệu vào SQL Server: {e}")

    finally:
        if 'conn' in locals():
            conn.close()

if __name__ == "__main__":
    print("🔄 Đang import dữ liệu từ CSV vào SQL Server...")
    import_csv_to_sql()
