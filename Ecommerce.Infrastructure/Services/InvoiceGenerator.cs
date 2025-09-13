using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Domain.Entities;
using System.Text;

namespace Ecommerce.Infrastructure.Services
{
    public class InvoiceGenerator : IInvoiceGenerator
    {
        public string GenerateInvoiceHtml(Orders order, InforDTO customerInfo, Discounts? discount)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (customerInfo == null) throw new ArgumentNullException(nameof(customerInfo));

            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append($@"
        <!doctype html>
        <html lang=""vi"">
        <head>
          <meta charset=""utf-8"" />
          <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
          <title>Hóa đơn bán hàng - Doris</title>
          <style>
            /* Reset */
            *{{box-sizing:border-box}}
            body{{font-family: Arial, Helvetica, sans-serif;margin:0;padding:20px;color:#222;background:#f5f6f8}}
            .invoice-wrap{{max-width:900px;margin:0 auto;background:#fff;padding:28px;border-radius:8px;box-shadow:0 6px 18px rgba(0,0,0,0.06)}}
            header{{display:flex;justify-content:space-between;align-items:center;margin-bottom:18px}}
            .brand{{display:flex;gap:14px;align-items:center}}
            .brand img{{width:88px;height:88px;object-fit:contain;border-radius:6px;background:#fff;padding:6px;border:1px solid #eee}}
            .brand .name{{font-size:20px;font-weight:700}}
            .meta{{font-size:13px;text-align:right}}
            .meta .title{{font-weight:700;margin-bottom:6px}}

            .info{{display:flex;justify-content:space-between;gap:20px;margin-bottom:18px}}
            .info .box{{flex:1;padding:12px;border-radius:6px;border:1px dashed #e6e6e6;background:#fafafa}}
            h2.section{{font-size:14px;margin:0 0 8px 0}}

            table{{width:100%;border-collapse:collapse;margin-top:12px}}
            table thead th{{background:#f3f4f6;padding:10px;border-bottom:1px solid #e8e8e8;text-align:left;font-size:13px}}
            table tbody td{{padding:10px;border-bottom:1px solid #f0f0f0;font-size:13px}}
            .right{{text-align:right}}

            .totals{{margin-top:14px;display:flex;justify-content:flex-end}}
            .totals .tbl{{width:360px}}
            .totals .tbl table{{width:100%}}
            .totals .tbl td{{padding:8px}}
            .totals .tbl tr.total td{{font-weight:800;font-size:16px;border-top:2px solid #ddd}}

            .notes{{margin-top:18px;font-size:13px}}
            .actions{{display:flex;gap:8px;justify-content:flex-end;margin-top:18px}}
            .btn{{padding:10px 14px;border-radius:6px;border:0;cursor:pointer;font-size:13px}}
            .btn-print{{background:#2563eb;color:#fff}}
            .btn-download{{background:#10b981;color:#fff}}
            .btn-close{{background:#dc2626;color:#fff}}

            footer{{font-size:12px;color:#666;margin-top:18px;text-align:center}}

            /* Print styles */
            @media print{{
              body{{background:transparent;padding:0}}
              .actions{{display:none}}
              .invoice-wrap{{box-shadow:none;margin:0;border-radius:0;padding:20px}}
              @page {{ margin: 1cm; }}
            }}

            /* Loading animation */
            .loading{{display:none;position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);background:rgba(0,0,0,0.8);color:white;padding:20px;border-radius:8px;z-index:1000}}
            .loading.show{{display:block}}
          </style>
        </head>
        <body>
          <div class=""loading"" id=""loadingIndicator"">Đang chuẩn bị in...</div>
          
          <div class=""invoice-wrap"" id=""invoice"">
            <header>
              <div class=""brand"">
                <img src=""https://iili.io/KTETTWQ.jpg"" alt=""Logo Doris"" id=""brandLogo"">
                <div>
                  <div class=""name"">DORIS - Thời trang</div>
                  <div style=""font-size:13px;color:#555"">Thời trang nữ &amp; nam</div>
                </div>
              </div>
              <div class=""meta"">
                <div class=""title"">HÓA ĐƠN BÁN HÀNG</div>
                <div>Mã hóa đơn: <strong id=""invoiceNo"">{order.OrderID}</strong></div>
                <div>Ngày: <strong id=""invoiceDate"">{order.OrderDate:dd/MM/yyyy}</strong></div>
              </div>
            </header>

            <section class=""info"">
              <div class=""box"">
                <h2 class=""section"">Thông tin cửa hàng</h2>
                <div><strong>DORIS</strong></div>
                <div>MST: <span id=""sellerTax"">0123456789</span></div>
                <div>Địa chỉ: <span id=""sellerAddress"">23 Ngô Quyền, Long Thành, Đồng Nai</span></div>
                <div>Điện thoại: <span id=""sellerPhone"">(028) 1234 5678</span></div>
                <div>Email: <span id=""sellerEmail"">info@doris.vn</span></div>
                <div>Website: <span id=""sellerWebsite"">www.doris.vn</span></div>
              </div>

              <div class=""box"">
                <h2 class=""section"">Thông tin khách hàng</h2>
                <div>Họ tên: <strong id=""buyerName"">{customerInfo.UserName}</strong></div>
                <div>Địa chỉ: <span id=""buyerAddress"">{order.Shippings.First().ShippingAddress ?? "N/A"}</span></div>
                <div>Điện thoại: <span id=""buyerPhone"">{order.Shippings.First().TrackingNumber ?? "N/A"}</span></div>
                <div>Email: <span id=""buyerEmail"">{customerInfo.Email}</span></div>
                <div>Ghi chú: <span id=""buyerNote"">Giao hàng giờ hành chính</span></div>
              </div>
            </section>

            <section>
              <table>
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Sản phẩm</th>
                    <th>Màu/SIZE</th>
                    <th class=""right"">SL</th>
                    <th class=""right"">Đơn giá (₫)</th>
                    <th class=""right"">CK/Chiết khấu (₫)</th>
                    <th class=""right"">Thành tiền (₫)</th>
                  </tr>
                </thead>
                <tbody id=""items"">");

            // Thêm các dòng sản phẩm từ OrderDetails
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                int itemNumber = 1;
                double subTotal = 0;

                foreach (var item in order.OrderDetails)
                {
                    double itemTotal = item.UnitPrice * item.Quantity;
                    subTotal += itemTotal;

                    htmlBuilder.Append($@"
                  <tr>
                    <td>{itemNumber++}</td>
                    <td>{item.Products?.ProductName ?? "Sản phẩm không xác định"}</td>
                    <td>{item.ProductSizes?.ProductColors?.ColorName ?? "N/A"} / {item.ProductSizes?.Size ?? "N/A"}</td>
                    <td class=""right"">{item.Quantity}</td>
                    <td class=""right"">{item.UnitPrice.ToString("N0")}</td>
                    <td class=""right"">0</td>
                    <td class=""right"">{itemTotal.ToString("N0")}</td>
                  </tr>");
                }

                // Tính toán tổng tiền
                double shippingFee = 30000;
                double discountValue = 0;
                if (discount != null)
                {
                    if (discount.DiscountType == 1) // FixedAmount
                    {
                        discountValue = discount.DiscountValue;
                    }
                    else if (discount.DiscountType == 2) // Percentage
                    {
                        discountValue = subTotal * (discount.DiscountValue / 100.0);
                    }

                    if (discountValue > subTotal) discountValue = subTotal;
                }
                double taxAmount = 0;
                double grandTotal = subTotal - discountValue + shippingFee + taxAmount;

                htmlBuilder.Append($@"
                </tbody>
              </table>

              <div class=""totals"">
                <div class=""tbl"">
                  <table>
                    <tr>
                      <td>Tạm tính</td>
                      <td class=""right"" id=""subTotal"">{subTotal.ToString("N0")}</td>
                    </tr>");

                if (discountValue > 0)
                {
                    htmlBuilder.Append($@"
                    <tr>
                      <td>Giảm giá</td>
                      <td class=""right"" id=""discount"">-{discountValue.ToString("N0")}</td>
                    </tr>");
                }

                htmlBuilder.Append($@"
                    <tr>
                      <td>Phí vận chuyển</td>
                      <td class=""right"" id=""shippingFee"">{shippingFee.ToString("N0")}</td>
                    </tr>
                    <tr>
                      <td>Thuế (VAT 10%)</td>
                      <td class=""right"" id=""taxAmount"">{taxAmount.ToString("N0")}</td>
                    </tr>
                    <tr class=""total"">
                      <td>Tổng cộng</td>
                      <td class=""right"" id=""grandTotal"">{grandTotal.ToString("N0")}</td>
                    </tr>
                    <tr>
                      <td colspan=""2"" style=""font-size:12px;padding-top:4px"">Bằng chữ: <em id=""totalInWords"">{NumberToVietnameseWords(grandTotal)}</em></td>
                    </tr>
                  </table>
                </div>
              </div>");
            }
            else
            {
                htmlBuilder.Append(@"
                  <tr>
                    <td colspan='7' style='text-align: center;'>Không có sản phẩm nào trong đơn hàng</td>
                  </tr>
                </tbody>
              </table>");
            }

            htmlBuilder.Append($@"
            </section>

            <div class=""notes"">
              <strong>Phương thức thanh toán:</strong> <span id=""paymentMethod"">{order.PaymentMethods?.MethodName ?? "N/A"}</span><br>
              <strong>Trạng thái:</strong> {GetStatusText(order.Status)}<br>
              <strong>Lưu ý:</strong> Hàng đã bán không đổi trả (nếu không còn phiếu bảo hành). Vui lòng kiểm tra kỹ khi nhận hàng.
            </div>

            <div style=""display:flex;justify-content:space-between;align-items:center;margin-top:20px"">
              <div>
                <div style=""font-size:13px;margin-bottom:36px"">Người lập hóa đơn</div>
                <div style=""font-weight:700"">(Ký, ghi rõ họ tên)</div>
              </div>
              <div style=""text-align:right"">
                <div style=""font-size:13px;margin-bottom:6px"">Người mua hàng</div>
                <div style=""font-weight:700"">(Ký, ghi rõ họ tên)</div>
              </div>
            </div>

            <div class=""actions"">
              <button class=""btn btn-print"" onclick=""printInvoice()"">🖨️ In hóa đơn</button>
              <button class=""btn btn-download"" onclick=""downloadInvoice()"">💾 Tải HTML</button>
              <button class=""btn btn-close"" onclick=""window.close()"">✖️ Đóng</button>
            </div>

            <footer>
              Hóa đơn được lập theo quy định. Mọi thông tin liên hệ: <span id=""sellerPhoneFooter"">(028) 1234 5678</span> - Email: <span id=""sellerEmailFooter"">info@doris.vn</span>
            </footer>
          </div>

          <script>
            // Chuyển đổi số thành chữ tiếng Việt
            function numberToVietnamese(n) {{
              const chu = [""không"",""một"",""hai"",""ba"",""bốn"",""năm"",""sáu"",""bảy"",""tám"",""chín""];
              if (n===0) return ""không"";
              function read3(num) {{
                let tram = Math.floor(num/100);
                let chuc = Math.floor((num%100)/10);
                let don = num%10;
                let res = """";
                if (tram>0) res += chu[tram]+"" trăm"";
                if (chuc>1) {{
                  res += (res?"" "":"""") + chu[chuc] + "" mươi"";
                  if (don===1) res += "" mốt"";
                  else if (don===5) res += "" lăm"";
                  else if (don>0) res += "" ""+chu[don];
                }} else if (chuc===1){{
                  res += (res?"" "":"""") + ""mười"";
                  if (don>0){{
                    if (don===5) res += "" lăm"";
                    else res += "" ""+chu[don];
                  }}
                }} else if (chuc===0 && don>0){{
                  if (tram>0) res += "" lẻ ""+chu[don];
                  else res += chu[don];
                }}
                return res;
              }}
              const units = ["""","" nghìn"","" triệu"","" tỷ""];
              let i=0;let out=[];
              while(n>0){{
                out.push(read3(n%1000) + units[i]);
                n = Math.floor(n/1000); i++;
              }}
              return out.filter(x=>x).reverse().join(' ').replace(/ +/g,' ').trim() + ' đồng';
            }}

            // Hàm in hóa đơn
            function printInvoice() {{
              const loading = document.getElementById('loadingIndicator');
              loading.classList.add('show');
              
              setTimeout(() => {{
                loading.classList.remove('show');
                window.print();
              }}, 500);
            }}

            // Hàm tải file HTML
            function downloadInvoice() {{
              const html = '<!doctype html>\\n' + document.documentElement.outerHTML;
              const blob = new Blob([html], {{type: 'text/html;charset=utf-8'}});
              const url = URL.createObjectURL(blob);
              const a = document.createElement('a');
              a.href = url; 
              a.download = 'HoaDon_{order.OrderID}.html';
              document.body.appendChild(a); 
              a.click(); 
              a.remove();
              URL.revokeObjectURL(url);
            }}

            // Tự động focus vào cửa sổ mới
            window.onload = function() {{
              window.focus();
              
              // Thêm keyboard shortcuts
              document.addEventListener('keydown', function(e) {{
                if (e.ctrlKey && e.key === 'p') {{
                  e.preventDefault();
                  printInvoice();
                }}
                if (e.key === 'Escape') {{
                  window.close();
                }}
              }});
            }};

            // Ngăn không cho đóng tab khi đang in
            let isPrinting = false;
            window.addEventListener('beforeprint', () => isPrinting = true);
            window.addEventListener('afterprint', () => isPrinting = false);
            
            window.addEventListener('beforeunload', (e) => {{
              if (isPrinting) {{
                e.preventDefault();
                e.returnValue = '';
              }}
            }});
          </script>
        </body>
        </html>");

            return htmlBuilder.ToString();
        }

        private string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Chờ xử lý",
                1 => "Lỗi",
                2 => "Hoàn thành",
                3 => "Đã hủy",
                4 => "Đã xác nhận",
                _ => "Không xác định"
            };
        }

        private string NumberToVietnameseWords(double number)
        {
            string[] units = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ", "tỷ tỷ" };
            string[] numbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            string str = number.ToString("F0");
            int i = str.Length;
            string result = "";
            int unitIndex = 0;

            while (i > 0)
            {
                int start = i - 3 > 0 ? i - 3 : 0;
                string part = str.Substring(start, i - start);
                int n = int.Parse(part);

                if (n > 0)
                {
                    string partStr = ReadThreeDigits(n);
                    result = partStr + " " + units[unitIndex] + " " + result;
                }

                unitIndex++;
                i -= 3;
            }

            return result.Trim() + " đồng";
        }

        private string ReadThreeDigits(int number)
        {
            string[] numbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] positions = { "", "mươi", "trăm" };

            string result = "";
            int digit1 = number / 100; // hàng trăm
            int digit2 = (number % 100) / 10; // hàng chục
            int digit3 = number % 10; // hàng đơn vị

            if (digit1 > 0)
            {
                result += numbers[digit1] + " trăm ";
            }

            if (digit2 > 0)
            {
                if (digit2 == 1)
                {
                    result += "mười ";
                }
                else
                {
                    result += numbers[digit2] + " mươi ";
                }
            }
            else if (digit1 > 0 && digit3 > 0)
            {
                result += "lẻ ";
            }

            if (digit3 > 0)
            {
                if (digit2 > 1 && digit3 == 1)
                {
                    result += "mốt";
                }
                else if (digit2 > 0 && digit3 == 5)
                {
                    result += "lăm";
                }
                else
                {
                    result += numbers[digit3];
                }
            }

            return result.Trim();
        }
    }
}
