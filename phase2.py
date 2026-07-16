import random
import matplotlib.pyplot as plt
nguyen_lieu_pho = 23000
gia_pho = 40000
nguyen_lieu_banh_mi = 10000
gia_banh_mi = 20000
chi_phi_xang = 2000
cong_ship = 15000
ti_le_hong_xe = 0.05  
chi_phi_sua_xe = 80000
chi_phi_sinh_hoat = 80000   
luong_npc = 100000
hoa_hong_shipper = 0.3
tien_mat_bang = 50000

#trang thai ban dau cua nguoi choi
#A: npc dung quan
#B: thue shipper di ship
current_cash_A = 50000
current_cash_B = 50000
day = 0
cash_history_A = [current_cash_A]
cash_history_B = [current_cash_B]
days_history = [day]

for i in range(30):
    day += 1
    days_history.append(day)
    daily_profit_A = 0
    daily_profit_B = 0
    tien_ship_thu_ve = 0
    so_don_pho = random.randint(12, 16)
    so_don_pho_tai_quan = int(so_don_pho * 0.6)
    so_don_pho_online = so_don_pho - so_don_pho_tai_quan
    so_don_banh_mi = random.randint(12, 16)
    so_don_bm_tai_quan = int(so_don_banh_mi * 0.6)
    so_don_bm_online = so_don_banh_mi - so_don_bm_tai_quan
    tong_don_online = so_don_bm_online + so_don_pho_online
    lai_tai_quan = so_don_pho_tai_quan * (gia_pho - nguyen_lieu_pho) + so_don_bm_tai_quan * (gia_banh_mi - nguyen_lieu_banh_mi)
    #truong hop thue npc ship
    lai_online = so_don_pho_online * (gia_pho - nguyen_lieu_pho) + so_don_bm_online * (gia_banh_mi - nguyen_lieu_banh_mi)
    for j in range(tong_don_online):
        tip = random.randint(2000, 8000)
        tien_ship_thu_ve += (cong_ship * (1-hoa_hong_shipper)) + tip
    daily_profit_A = tien_ship_thu_ve + lai_online + lai_tai_quan
    daily_profit_A -= tien_mat_bang + chi_phi_sinh_hoat
    
    current_cash_A += daily_profit_A
    cash_history_A.append(current_cash_A)

    #truong hop tu di ship
    pho_tai_quan_B = random.randint(10, 12)
    bm_tai_quan_B = random.randint(10, 12)
    lai_tai_quan_B = pho_tai_quan_B * (gia_pho - nguyen_lieu_pho) + bm_tai_quan_B * (gia_banh_mi - nguyen_lieu_banh_mi)
    lai_ship = 0
    so_don_pho_di_ship = random.randint(5, 8)
    so_don_banh_mi_di_ship = random.randint(5, 8)
    for k in range(so_don_pho_di_ship):
        tip = random.randint(2000, 8000)
        loi_nhuan_1_don = tip + cong_ship + (gia_pho - nguyen_lieu_pho) - chi_phi_xang
        if random.random() < ti_le_hong_xe:
            loi_nhuan_1_don -= chi_phi_sua_xe
        lai_ship += loi_nhuan_1_don
    for l in range(so_don_banh_mi_di_ship):
        tip = random.randint(2000,8000)
        loi_nhuan_1_don = tip + cong_ship + (gia_banh_mi - nguyen_lieu_banh_mi) - chi_phi_xang
        if random.random() < ti_le_hong_xe:
            loi_nhuan_1_don -= chi_phi_sua_xe
        lai_ship += loi_nhuan_1_don
    daily_profit_B = lai_ship + lai_tai_quan_B
    daily_profit_B -= tien_mat_bang + chi_phi_sinh_hoat + luong_npc

    current_cash_B += daily_profit_B
    cash_history_B.append(current_cash_B)
    
plt.figure(figsize=(12, 6))
plt.plot(days_history, cash_history_A, marker='o', color='blue', label='tự nấu và thuê npc ship')
plt.text(day + 0.2, current_cash_A, f"{current_cash_A:,.0f} VNĐ", fontsize=10, fontweight='bold', color='darkred', va='center')
plt.plot(days_history, cash_history_B, marker='s', color='orange', label='tự nấu và tự ship')
plt.text(day + 0.2, current_cash_B, f"{current_cash_B:,.0f} VNĐ", fontsize=10, fontweight='bold', color='darkred', va='center')
plt.title('biểu đồ giai đoạn 2', fontsize=14, fontweight='bold')
plt.xlabel('day', fontsize=12)
plt.ylabel('tai sản tích lũy', fontsize=12)
plt.legend(loc='upper left')
plt.grid(True, linestyle=':', alpha=0.6)
plt.gca().get_yaxis().set_major_formatter(plt.FuncFormatter(lambda x, loc: "{:,}".format(int(x))))
plt.show()   