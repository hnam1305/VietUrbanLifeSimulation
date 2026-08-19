import random
import matplotlib.pyplot as plt

#import cac thong so tu excel
nguyen_lieu_pho = 23000
gia_pho = 40000
nguyen_lieu_banh_mi = 10000
gia_banh_mi = 20000
cong_ship = 15000
chi_phi_xang = 2000
ti_le_hong_xe = 0.05  
chi_phi_sua_xe = 80000
chi_phi_sinh_hoat = 80000
von_de_mo_quan = 3000000

#trang thai ban dau cua nguoi choi
current_cash = 0  
day = 0           
cash_history = [current_cash]  
days_history = [day]           

#simulate each day 
while current_cash < von_de_mo_quan:
    day += 1
    daily_profit = 0
    so_don_pho = random.randint(4, 6)      #random 4-6 bat pho
    so_don_banh_mi = random.randint(4, 6)  #random 4-6 cai banh mi
    for _ in range(so_don_pho):
        tip = random.randint(2000, 8000) #random tien tip từ 2-8k
        #tính lai pho
        revenue = gia_pho + cong_ship + tip
        cost = nguyen_lieu_pho + chi_phi_xang
        profit = revenue - cost
        if random.random() < ti_le_hong_xe:
            profit -= chi_phi_sua_xe
        daily_profit += profit

    for _ in range(so_don_banh_mi):
        tip = random.randint(2000, 8000)
        #tính lãi bánh mì
        revenue = gia_banh_mi + cong_ship + tip
        cost = nguyen_lieu_banh_mi + chi_phi_xang
        profit = revenue - cost
        if random.random() < ti_le_hong_xe:
            profit -= chi_phi_sua_xe
            
        daily_profit += profit
    daily_profit -= chi_phi_sinh_hoat
    current_cash += daily_profit
    cash_history.append(current_cash)
    days_history.append(day)
plt.figure(figsize=(10, 6))
plt.plot(days_history, cash_history, marker='o', color='g', label='tiền tích lũy được')
#line đỏ mốc 3m để mở quán
plt.axhline(y=von_de_mo_quan, color='r', linestyle='--', label='mốc mở quán (3m)')
#marker đánh dấu ngày đạt mốc (AI)
plt.plot(day, current_cash, marker='*', color='gold', markersize=15, label='ngày mở quán')
#AI hiển thị tiền kiếm được sau {day} ngày
plt.text(day + 0.2, current_cash, f"{current_cash:,.0f} VNĐ", fontsize=10, fontweight='bold', color='darkred', va='center')
plt.title('Biểu đồ giai đoạn 1', fontsize=14, fontweight='bold')
plt.xlabel('Day', fontsize=12)
plt.ylabel('tiền tích lũy được', fontsize=12)
#grid (AI)
plt.grid(True, linestyle=':', alpha=0.6)
#legend(AI)
plt.legend(loc='upper left')
#format tiền (AI)
plt.gca().get_yaxis().set_major_formatter(plt.FuncFormatter(lambda x, loc: "{:,}".format(int(x))))
plt.show()