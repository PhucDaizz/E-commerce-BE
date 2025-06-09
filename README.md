# 🛒 E-commerce Platform - Full-Stack Solution

<div align="center">

[![GitHub stars](https://img.shields.io/github/stars/PhucDaizz/E-commerce-BE?style=for-the-badge&logo=github)](https://github.com/PhucDaizz/E-commerce-BE/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/PhucDaizz/E-commerce-BE?style=for-the-badge&logo=github)](https://github.com/PhucDaizz/E-commerce-BE/network/members)
[![GitHub issues](https://img.shields.io/github/issues/PhucDaizz/E-commerce-BE?style=for-the-badge&logo=github)](https://github.com/PhucDaizz/E-commerce-BE/issues)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

**A modern, full-featured e-commerce platform built with ASP.NET Core and ReactJS**

[🚀 Live Demo](https://github.com/PhucDaizz/E-commerce-FE) • [📖 Documentation](#-getting-started) • [🐛 Report Bug](https://github.com/PhucDaizz/E-commerce-BE/issues) • [✨ Request Feature](https://github.com/PhucDaizz/E-commerce-BE/issues)

</div>

---

## 📋 Table of Contents

- [🎯 Overview](#-overview)
- [✨ Features](#-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [🏗️ Architecture](#️-architecture)
- [🚀 Getting Started](#-getting-started)
- [📡 API Endpoints](#-api-endpoints)
- [📸 Screenshots](#-screenshots)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

---

## 🎯 Overview

This is a **comprehensive e-commerce platform** designed and developed from the ground up, featuring both frontend and backend components. The platform provides a complete online shopping experience with modern features including real-time chat support, secure payment processing, and robust admin management capabilities.

### 🌟 What Makes This Special?

- **💯 Full-Stack Ownership**: Complete control over both frontend and backend
- **🔒 Enterprise-Grade Security**: JWT authentication with ASP.NET Core Identity
- **⚡ Real-Time Features**: Live chat support with SignalR
- **💳 Secure Payments**: Integrated VNPAY payment gateway
- **📱 Responsive Design**: Mobile-first approach with Bootstrap
- **🚀 Modern Architecture**: Clean, scalable layered architecture

---

## ✨ Features

<div align="center">

| 🛍️ **E-commerce Core** | 💬 **Communication** | 🔐 **Security** | ⚙️ **Management** |
|:---:|:---:|:---:|:---:|
| Product Catalog | Live Chat Support | JWT Authentication | Admin Dashboard |
| Shopping Cart | Real-time Notifications | Secure Payments | Order Management |
| Order Tracking | Email Automation | User Authorization | Inventory Control |
| Payment Gateway | Background Services | Data Protection | User Administration |

</div>

### 🛍️ **Customer Experience**
- 📦 **Product Management**: Comprehensive catalog with detailed product information
- 🛒 **Shopping Cart**: Persistent cart with real-time updates
- 📋 **Order Processing**: Seamless checkout and order tracking
- 💳 **Secure Payments**: VNPAY integration for safe transactions
- 💬 **Live Support**: Real-time chat assistance
- 📧 **Email Notifications**: Automated order confirmations and updates

### 👨‍💼 **Administrative Features**
- 📊 **Dashboard**: Comprehensive overview of business metrics
- 📦 **Product Management**: Full CRUD operations for inventory
- 🧾 **Order Management**: Order approval and status management
- 👥 **User Management**: Customer account administration
- 🚚 **Shipping Integration**: Carrier management and tracking
- 📈 **Analytics**: Business insights and reporting

---

## 🛠️ Tech Stack

<div align="center">

### Backend Technologies
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Sever-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)

### Frontend Technologies
![React](https://img.shields.io/badge/react-%2320232a.svg?style=for-the-badge&logo=react&logoColor=%2361DAFB)
![Vite](https://img.shields.io/badge/vite-%23646CFF.svg?style=for-the-badge&logo=vite&logoColor=white)
![Bootstrap](https://img.shields.io/badge/bootstrap-%23563D7C.svg?style=for-the-badge&logo=bootstrap&logoColor=white)
![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)

</div>

### 🔧 **Backend Stack**
```
🌐 ASP.NET Core 8.0 Web API
🗄️ Entity Framework Core 8.0
🛡️ ASP.NET Core Identity + JWT
⚡ SignalR for Real-time Communication
💳 VNPAY Payment Gateway
📧 MailKit for Email Services
🗺️ AutoMapper for Object Mapping
📚 Swagger/OpenAPI Documentation
```

### 🎨 **Frontend Stack**
```
⚛️ ReactJS with Vite
🎨 Bootstrap for Styling
📱 Responsive Design
🔄 Real-time Updates
```

---

## 🏗️ Architecture
![Alt text](https://iili.io/FFTo9tt.png)



### 🏛️ **Layered Architecture**

| Layer | Responsibility | Technologies |
|-------|---------------|-------------|
| **🖥️ Presentation** | API Controllers, HTTP handling | ASP.NET Core Controllers |
| **💼 Business Logic** | Core business rules, orchestration | Service Classes |
| **🗄️ Data Access** | Database operations, CRUD | Repository Pattern, EF Core |
| **⚡ Real-time** | Live communication | SignalR Hubs |
| **🔧 Background** | Async tasks, maintenance | Hosted Services |

---

## 🚀 Getting Started

### 📋 Prerequisites

Before you begin, ensure you have the following installed:

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- ✅ [Node.js](https://nodejs.org/) (v16 or higher)
- ✅ [Microsoft SQL Server](https://www.microsoft.com/sql-server)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### 🔧 Installation

#### 1️⃣ **Clone the Repositories**

```bash
# Clone backend repository
git clone https://github.com/PhucDaizz/E-commerce-BE.git

# Clone frontend repository
git clone https://github.com/PhucDaizz/E-commerce-FE.git
```

#### 2️⃣ **Backend Setup**

```bash
# Navigate to backend directory
cd E-commerce-BE

# Restore NuGet packages
dotnet restore

# Update database connection string in appsettings.json
# Configure JWT, VNPAY, and Email settings as needed

# Run database migrations
dotnet ef database update --project ECommerce.API

# Start the backend API
dotnet run --project ECommerce.API
```

**🌐 Backend will be available at:** `https://localhost:7295`  
**📚 Swagger Documentation:** `https://localhost:7295/swagger`

#### 3️⃣ **Frontend Setup**

```bash
# Navigate to frontend directory
cd E-commerce-FE

# Install dependencies
npm install

# Create environment configuration
echo "VITE_API_BASE_URL=https://localhost:7295" > .env

# Start the development server
npm run dev
```

**🌐 Frontend will be available at:** `http://localhost:5173`

### ⚙️ **Configuration**

#### 🔧 Backend Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string"
  },
  "JwtSettings": {
    "SecretKey": "Your JWT secret key",
    "Issuer": "Your issuer",
    "Audience": "Your audience"
  },
  "VnPaySettings": {
    "TmnCode": "Your VNPAY merchant code",
    "HashSecret": "Your VNPAY hash secret"
  },
  "EmailSettings": {
    "SmtpServer": "Your SMTP server",
    "SmtpPort": 587,
    "Username": "Your email",
    "Password": "Your email password"
  }
}
```

---

## 📡 API Endpoints

<details>
<summary><strong>🔐 Authentication Endpoints</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | User registration |
| `POST` | `/api/auth/login` | User login |
| `POST` | `/api/auth/refresh` | Refresh JWT token |
| `POST` | `/api/auth/logout` | User logout |

</details>

<details>
<summary><strong>🛍️ Product Endpoints</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{id}` | Get product by ID |
| `POST` | `/api/products` | Create new product |
| `PUT` | `/api/products/{id}` | Update product |
| `DELETE` | `/api/products/{id}` | Delete product |

</details>

<details>
<summary><strong>🧾 Order Endpoints</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/orders` | Get user orders |
| `GET` | `/api/orders/{id}` | Get order details |
| `POST` | `/api/orders` | Create new order |
| `PUT` | `/api/orders/{id}/status` | Update order status |

</details>

<details>
<summary><strong>💳 Payment Endpoints</strong></summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/payments/vnpay/create` | Create VNPAY payment |
| `GET` | `/api/payments/vnpay/callback` | VNPAY payment callback |

</details>

<details>
<summary><strong>⚡ Real-time Communication</strong></summary>

| Hub | Endpoint | Description |
|-----|----------|-------------|
| `ChatHub` | `/chathub` | SignalR chat functionality |

</details>

---

## 📸 Screenshots

<div align="center">

### 🏠 **Homepage**
![Homepage](https://via.placeholder.com/800x400/007acc/ffffff?text=Homepage+Screenshot)

### 🛒 **Shopping Cart**
![Shopping Cart](https://via.placeholder.com/800x400/28a745/ffffff?text=Shopping+Cart+Screenshot)

### 👨‍💼 **Admin Dashboard**
![Admin Dashboard](https://via.placeholder.com/800x400/dc3545/ffffff?text=Admin+Dashboard+Screenshot)

</div>

---

## 🔮 Roadmap

- [ ] 🌐 Multi-language support
- [ ] 📱 Mobile app development
- [ ] 🔍 Advanced search and filtering
- [ ] 📊 Advanced analytics dashboard
- [ ] 🤖 AI-powered product recommendations
- [ ] 📦 Multi-vendor marketplace features
- [ ] 🌍 International shipping integration
- [ ] 🎨 Theme customization

---

## 🤝 Contributing

We welcome contributions from the community! Here's how you can help:

### 🚀 **Getting Started with Contributing**

1. **🍴 Fork the repository**
2. **🌿 Create your feature branch**
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. **💾 Commit your changes**
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```
4. **📤 Push to the branch**
   ```bash
   git push origin feature/AmazingFeature
   ```
5. **🔄 Open a Pull Request**

### 📝 **Contribution Guidelines**

- 🧪 Write tests for new features
- 📚 Update documentation
- 🎨 Follow existing code style
- 🔍 Ensure code quality with linting
- 📝 Write clear commit messages

---

## 🙏 Acknowledgments

- 🎨 **Bootstrap** for the beautiful UI components
- ⚡ **SignalR** for real-time communication
- 💳 **VNPAY** for secure payment processing
- 📧 **MailKit** for reliable email services
- 🗺️ **AutoMapper** for object mapping
- 📚 **Swagger** for API documentation

---

## 📞 Contact & Support

<div align="center">

**👨‍💻 Developer:** PhucDaizz  
**📧 Email:** [dai742004.dn@gmail.com]  
**💼 LinkedIn:** [[Nguyễn Phúc Đại](https://www.linkedin.com/in/nguy%E1%BB%85n-ph%C3%BAc-%C4%91%E1%BA%A1i-82719a27b/)]  
**🐙 GitHub:** [@PhucDaizz](https://github.com/PhucDaizz)

**💡 Have questions?** [Create an issue](https://github.com/PhucDaizz/E-commerce-BE/issues/new)  
**🐛 Found a bug?** [Report it here](https://github.com/PhucDaizz/E-commerce-BE/issues/new?template=bug_report.md)  
**✨ Want a feature?** [Request it here](https://github.com/PhucDaizz/E-commerce-BE/issues/new?template=feature_request.md)

</div>

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

<div align="center">

**⭐ If you found this project helpful, please consider giving it a star!**

[![GitHub stars](https://img.shields.io/github/stars/PhucDaizz/E-commerce-BE?style=social)](https://github.com/PhucDaizz/E-commerce-BE/stargazers)

---

**Built with ❤️ by [PhucDaizz](https://github.com/PhucDaizz)**

</div>
