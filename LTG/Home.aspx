<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Vivify.Home" %>


<!DOCTYPE html>
<html lang="en">
<head>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/aos/2.3.4/aos.css">
<script src="https://cdnjs.cloudflare.com/ajax/libs/aos/2.3.4/aos.js"></script>

    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Vivify Soft</title>
      <link href="assets/img/favicon.ico" rel="icon">
      <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap');

 

 
html, body {
    margin: 0;
    padding: 0;
    height: 100%; 
    width: 100%;
    font-family: 'Poppins', sans-serif;
    overflow-x: hidden;
        scroll-behavior: smooth;
    scroll-padding-top: 80px; 
}

body {
    background: url('assets/img/vs-bg2.jpg') no-repeat center center fixed;
    background-size: cover;
    position: relative;
    min-height: 100vh; /* Ensures it fills the viewport */
}

/* Glassy Effect */
body::before {
    content: "";
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(255, 255, 255, 0.1);  
    backdrop-filter: blur(8px);  
    z-index: -1;
}


        h4 {
    font-size: 16px;
    color: #73A5FF;
}

h1 {
    font-size: 50px;
    font-weight: bold;
    line-height: 1.2;
}

h1 span {
    color: #2D8CFF;
}

p {
    margin-top: 15px;
    font-size: 18px;
    opacity: 0.8;
}

 .navbar {
    background-color: #f8f9fc;
    display: flex;
    justify-content: space-between;
    align-items: center;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    padding: 10px 20px; /* Reduced padding for better spacing */
    width: 100%;
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 1000;
    padding: 8px 20px;
    height: 80px; /* Fixed height */
    overflow: hidden;
}

        .logo img {
            height: 70px; /* Slightly bigger */
            width: auto;
            object-fit: contain;
           
        }


.nav-links {
    display: flex;
    gap: 20px; /* Reduced gap for a compact look */
}

.nav-links a {
    color: #003366;
    text-decoration: none;
    font-size: 17px;
    font-weight: bold;
    position: relative;
    padding: 5px 10px;
    transition: all 0.3s ease-in-out;
}

.nav-links a:hover {
    color: #007BFF;
    transform: scale(1.05);
}

.nav-links a::after {
    content: '';
    position: absolute;
    left: 0;
    bottom: 0;
    width: 0;
    height: 2px;
    background-color: #007BFF;
    transition: all 0.3s ease-in-out;
}

.nav-links a:hover::after {
    width: 100%;
}

.nav-links i {
    margin-right: 5px;
    transition: transform 0.3s ease-in-out;
    font-size: 14px;
    font-weight: bold;
}
.nav-links a:hover i {
    transform: rotate(360deg);
}
.custom-btn-primary {
    background-color: #007BFF;
    color: white;
    border: none;
    padding: 8px 15px;
    font-size: 14px;
    font-weight: bold;
    border-radius: 5px;
    cursor: pointer;
    transition: all 0.3s ease-in-out;
}

.custom-btn-primary:hover {
    background-color: #0056b3;
}

  .menu-icon {
    display: none; /* Hidden by default */
    font-size: 30px;
    cursor: pointer;
    margin-right: 50px;
    z-index: 1100;
}


        .close-icon {
    display: none; /* Initially hidden */
    position: absolute;
    top: 15px;
    right: 15px;
    font-size: 30px;
    cursor: pointer;
    padding: 10px;
    border: 2px solid transparent;
    border-radius: 50%;
    transition: all 0.3s ease-in-out;
    z-index: 1100; /* Ensure it's above other elements */
}

 
/* Login Button */
        .custom-btn-primary {
            background-color: #007bff;
            color: white;
            padding: 12px 24px;
            border: none;
            cursor: pointer;
            font-size: 16px;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            transition: all 0.3s ease;
            margin-right: 35px;
        }

.custom-btn-primary:hover {
    background-color: #0056b3;
    transform: scale(1.05);
}

/* Full-screen Popup Overlay */
.custom-popup-overlay {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

/* Popup Box */
.custom-popup {
    background: white;
    width: 350px;
    padding: 20px;
    border-radius: 10px;
    box-shadow: 0px 6px 12px rgba(0, 0, 0, 0.2);
    text-align: center;
    position: relative;
    animation: fadeIn 0.3s ease-in-out;
}

/* Popup Title */
.popup-title {
    font-size: 20px;
    margin-bottom: 20px;
    font-weight: bold;
}

/* Close Button */
.custom-close-btn {
    position: absolute;
    top: 10px;
    right: 15px;
    font-size: 24px;
    cursor: pointer;
    color: #555;
    transition: color 0.3s ease;
}

.custom-close-btn:hover {
    color: red;
}

/* Popup Content */
.popup-content {
    display: flex;
    flex-direction: column;
    gap: 15px;
}

/* Popup Items */
.custom-popup-item {
    background: #007bff;
    color: white;
    padding: 12px;
    border-radius: 5px;
    text-decoration: none;
    font-size: 16px;
    transition: background 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 10px;
}

.custom-popup-item:hover {
    background: #0056b3;
}

/* Fade-in Animation */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

/* Show Popup */
.show {
    display: flex;
}#particleCanvas {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 100%;
    height: 100%;
    pointer-events: none;
}

 
/* Hero Section Animation */
.hero {
    height: 100vh;
 
    background-size: cover;
    background-position: center;
    color: #fff;
    display: flex;
    align-items: center;
    text-align: center;
    justify-content: center;
    overflow: hidden;
}

.container {
    max-width: 1100px;
    margin: 0 auto;
    padding: 20px;
    opacity: 0;
    transform: translateY(30px);
    animation: fadeSlideIn 1s ease-out forwards;
}

@keyframes fadeSlideIn {
    from {
        opacity: 0;
        transform: translateY(30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.btns {
    margin-top: 30px;
    opacity: 0;
    transform: translateY(20px);
    animation: fadeSlideIn 1.2s ease-out forwards 0.3s;
}

.btn-primary, .btn-secondary {
    padding: 12px 30px;
    border-radius: 5px;
    cursor: pointer;
}

.btn-primary {
    background-color: #2D8CFF;
    color: #fff;
    border: none;
    margin-right: 15px;
}

.btn-secondary {
    background-color: transparent;
    color: #2D8CFF;
    border: 1px solid #2D8CFF;
}


.btns {
    margin-top: 30px;
}

.btn-primary {
    background-color: #2D8CFF;
    color: #fff;
    padding: 12px 30px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    margin-right: 15px;
}

.btn-secondary {
    background-color: transparent;
    color: #2D8CFF;
    padding: 12px 30px;
    border: 1px solid #2D8CFF;
    border-radius: 5px;
    cursor: pointer;
} 

#particleCanvas {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 100%;
    height: 100%;
    pointer-events: none;
}

/*mission-vission*/
.mission-vision-section {
   
  font-family: 'Poppins', sans-serif;
  text-align: center;
  padding: 80px 0;
  color: white;
}

/* Heading Section */
.mission-vision-heading h1 {
  font-size: 2.8rem;
  font-weight: 700;
  margin-bottom: 10px;
  color: #003366;
}

.mission-vision-heading p {
  font-size: 1.2rem;
  max-width: 600px;
  margin: 0 auto 50px;
  opacity: 0.9;
  color: #555;
}

/* Cards Container */
.mission-vision-container {
  display: flex;
  justify-content: center;
  gap: 50px;
  flex-wrap: wrap;
  padding: 20px;
}

/* Mission & Vision Cards */
.mission-card,
.vision-card {
  background: white;
  padding: 40px;
  border-radius: 15px;
  width: 350px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
  text-align: center;
  transition: transform 0.3s ease-in-out;
  background-color: #e9f5ff;
}

.mission-card:hover,
.vision-card:hover {
  transform: translateY(-10px);
}

/* Icon Styling */
.mission-icon,
.vision-icon {
  width: 70px;
  height: 70px;
  background: #007BFF;
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 32px;
  border-radius: 50%;
  margin: 0 auto 20px;
  box-shadow: 0 5px 15px rgba(0, 123, 255, 0.3);
}

/* Card Headings */
.mission-card h4,
.vision-card h4 {
  color: #003366;
  font-size: 1.5rem;
  margin-bottom: 12px;
}

/* Paragraphs */
.mission-card p,
.vision-card p {
  font-size: 1rem;
  color: #555;
  line-height: 1.6;
}
@media (max-width: 768px) {
  .mission-vision-section {
    padding: 40px 15px;
  }

  .mission-vision-heading h1 {
    font-size: 2rem;
  }

  .mission-vision-heading p {
    font-size: 1rem;
    margin-bottom: 30px;
  }

  .mission-card,
  .vision-card {
    width: 100%;
    max-width: 90%;
    padding: 20px;
  }

  .mission-icon,
  .vision-icon {
    width: 50px;
    height: 50px;
    font-size: 24px;
  }

  .mission-card h4,
  .vision-card h4 {
    font-size: 1.2rem;
  }

  .mission-card p,
  .vision-card p {
    font-size: 0.9rem;
    line-height: 1.4;
  }
}
/*about*/
.about-section {
  display: flex;
  align-items: center;
  gap: 40px;
  flex-wrap: wrap;
  border-radius: 10px;
  overflow: hidden;
  background-color: #e6f5ff; /* Light blue background */
  padding: 40px; /* ✅ Added padding for desktop */
}

/* Mobile view styles */
@media (max-width: 768px) {
  .about-section {
    flex-direction: column; /* Stack vertically */
    gap: 20px;
    padding: 20px; /* ✅ Smaller padding for mobile */
    text-align: center;
  }

  .about-section h1 {
    font-size: 1.5rem;
  }

  .about-section p {
    font-size: 14px;
    line-height: 1.6;
  }

  .about-section img {
    max-width: 90%;
    margin: auto;
  }
}

.about-section {
  display: flex;
  align-items: center;
  gap: 40px;
  flex-wrap: wrap;
  border-radius: 10px;
  overflow: hidden;
  background-color: #e6f5ff; /* Light blue background */
}

/* Mobile view styles */
@media (max-width: 768px) {
  .about-section {
    flex-direction: column; /* Stack image & text vertically */
    gap: 20px;
    padding: 15px;
    text-align: center; /* Center text on small screens */
  }

  .about-section h1 {
    font-size: 1.5rem; /* Smaller heading */
  }

  .about-section p {
    font-size: 14px; /* Smaller text */
    line-height: 1.6;
  }

  .about-section img {
    max-width: 90%;
    margin: auto;
  }
}



 
 
 
/* Product & Services Section */

.product/service{
        font-family: 'Poppins', sans-serif;
    background-color: #f4f4f9;
    margin: 0;
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
}

.products-container {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 25px;
    padding: 40px;
    max-width: 1200px;
}

/* Product Card */
.product-card {
    background: white;
    padding: 30px;
    text-align: center;
    border-radius: 12px;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
    transition: transform 0.3s ease, box-shadow 0.3s ease;
    position: relative;
    overflow: hidden;
}

/* Large Card */
.large-card {
    min-height: 200px;
}

 

/* Icons */
.product-icon-wrapper {
    background: #007bff1a;
    padding: 15px;
    border-radius: 50%;
    display: inline-block;
    margin-bottom: 15px;
    transition: background 0.3s ease;
}

.product-icon {
    font-size: 38px;
    color: #007bff;
    transition: color 0.3s ease;
}

/* Title */
.product-title {
    font-size: 22px;
    font-weight: bold;
    margin: 10px 0;
}

/* Description */
        .product-description {
            font-size: 16px;
            color: #555;
            line-height: 1.5;
            text-align: center;
        }

/* Hover Effects - Change Background & Text Color */
.product-card:hover {
    background-color: #007bff; /* Blue background */
    color: white; /* White text */
    transform: translateY(-8px);
    box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
}

/* Ensure all text inside the card changes to white */
.product-card:hover .product-title,
.product-card:hover .product-description {
    color: white;
}

/* Change the icon background on hover */
.product-card:hover .product-icon-wrapper {
    background: white; /* White background for contrast */
}

/* Change the icon color on hover */
.product-card:hover .product-icon {
    color: #007bff; /* Blue icon color */
}

/* Clients Section */
/* General Styling */
.clients-section {
    font-family: 'Poppins', sans-serif;
    padding: 80px 5%;
 
}

/* Grid Layout for Modern Look */
.clients-container {
    display: grid;
    grid-template-columns: 1fr 1.5fr;
    align-items: center;
    gap: 50px;
    max-width: 1200px;
    margin: 0 auto;
}

/* Image Styling */
.clients-image img {
    max-width: 100%;
    height: auto;
    border-radius: 15px;
    box-shadow: 0px 10px 30px rgba(0, 0, 0, 0.15);
    padding: 10px;
    background: white;
    transition: transform 0.3s ease;
}

.clients-image img:hover {
    transform: scale(1.05);
}

/* Content Styling */
.clients-content h1 {
    font-size: 2.5rem;
    font-weight: 700;
    color: white;
    margin-bottom: 20px;
}

        .clients-content p {
            font-size: 1.2rem;
            color:    white;
            line-height: 1.6;
            margin-bottom: 25px;
            text-align: left;
            font-weight:bold;
             
        }

/* List Styling */
.clients-list {
    list-style: none;
    padding: 0;
    display: grid;
    gap: 12px;
}

.clients-list li {
    font-size: 1.1rem;
    color: #222;
    font-weight: 500;
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 10px;
    border-radius: 8px;
    background: white;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
    transition: all 0.3s ease;
}

.clients-list li:hover {
    transform: translateY(-3px);
    box-shadow: 0px 6px 15px rgba(0, 0, 0, 0.15);
}

/* Icon Styling */
.clients-list li::before {
    content: "✔️";
    font-size: 1.4rem;
    color: #007bff;
}


/* Contact Section Styling */
.contact-section {
    background: url('your-background-image.jpg') no-repeat center center/cover;
    padding: 80px 5%;
    display: flex;
    justify-content: center;
}

/* Contact Container */
.contact-container {
    display: flex;
    gap: 50px;
    max-width: 1100px;
    width: 100%;
    background: rgba(255, 255, 255, 0.15);
    backdrop-filter: blur(10px);
    padding: 40px;
    border-radius: 12px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

/* Form & Info Box */
.contact-form, .contact-info {
    flex: 1;
    padding: 20px;
}

/* Heading */
.contact-form h2, .contact-info h2 {
    font-size: 2rem;
    color: #003366;
    margin-bottom: 20px;
    font-weight: 700;
}

/* Input Fields */
.input-group {
    margin-bottom: 15px;
}

.input-group input, 
.input-group textarea {
    width: 100%;
    padding: 12px;
    font-size: 1rem;
    border-radius: 8px;
    border: 1px solid #ddd;
    outline: none;
    transition: all 0.3s ease;
}

.input-group input:focus, 
.input-group textarea:focus {
    border-color: #007bff;
    box-shadow: 0px 4px 10px rgba(0, 123, 255, 0.2);
}

/* Send Button */
.send-btn {
    width: 100%;
    padding: 12px;
    font-size: 1.2rem;
    font-weight: bold;
    color: white;
    background: #007bff;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    transition: 0.3s ease;
}

.send-btn:hover {
    background: #0056b3;
}

/* Contact Info */
.contact-info p {
    font-size: 1.1rem;
    margin-bottom: 10px;
}

.info-link {
    font-size: 1.1rem;
    color: #007bff;
    font-weight: bold;
    text-decoration: none;
    display: inline-block;
    margin-top: 10px;
}

.info-link:hover {
    text-decoration: underline;
}
/* Contact Social Icons */
.contact-social-icons {
    margin-top: 20px;
    display: flex;
    gap: 15px;
}

 

 

 

 
/* Chat Assistant */
.chat-assistant {
    margin-top: 20px;
    padding: 15px;
    background: rgba(255, 255, 255, 0.2);
    border-radius: 10px;
    backdrop-filter: blur(10px);
    box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.2);
}

.chat-box {
    max-height: 150px;
    overflow-y: auto;
    padding: 10px;
    border-radius: 8px;
    background: rgba(255, 255, 255, 0.1);
    margin-bottom: 10px;
}

.bot-message, .user-message {
    padding: 8px 12px;
    margin: 5px 0;
    border-radius: 5px;
    display: inline-block;
}

.bot-message {
    background: #007bff;
    color: white;
    align-self: flex-start;
}

.user-message {
    background: #34c759;
    color: white;
    align-self: flex-end;
}

.chat-input {
    display: flex;
    gap: 10px;
}

.chat-input input {
    flex: 1;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 5px;
}

.chat-input button {
    padding: 8px 12px;
    background: #007bff;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
}

.chat-input button:hover {
    background: #0056b3;
}

/*footer*/
.footer {
    background-color: #0a2a5e; /* Dark blue background */
    color: white;
    padding: 50px 20px;
    text-align: center;
}

.footer-container {
    display: flex;
    justify-content: space-around;
    align-items: flex-start;
    max-width: 1200px;
    margin: 0 auto;
    flex-wrap: wrap;
    gap: 30px;
}

.footer-logo {
    flex: 1;
    min-width: 350px;
    text-align: left;
}

.footer-logo h2 {
    font-size: 26px;
    margin-bottom: 15px;
}

.footer-logo p {
    font-size: 15px;
    line-height: 1.6;
}

.footer-links, .footer-contact {
    flex: 1;
    min-width: 250px;
    text-align: left;
}

.footer-links h3, .footer-contact h3 {
    font-size: 18px;
    margin-bottom: 15px;
    position: relative;
}

.footer-links h3::after, .footer-contact h3::after {
    content: "";
    display: block;
    width: 40px;
    height: 2px;
    background: white;
    margin-top: 5px;
}

.footer-links ul {
    list-style: none;
    padding: 0;
}

.footer-links ul li {
    margin: 10px 0;
}

.footer-links ul li a {
    color: white;
    text-decoration: none;
    transition: color 0.3s;
}

.footer-links ul li a:hover {
    color: #f8c102; /* Highlight color */
}

.footer-contact p {
    margin: 10px 0;
    display: flex;
    align-items: center;
    gap: 10px;
}

.footer-contact i {
    color: #f8c102;
    font-size: 18px;
}

.footer-bottom {
    text-align: center;
    margin-top: 30px;
    padding-top: 20px;
    border-top: 1px solid white;
    font-size: 14px;
}

 
 
 
 
.app-popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;              /* enable flexbox */
  justify-content: center;    /* center horizontally */
  align-items: center;        /* center vertically */
  background: rgba(0, 0, 0, 0.6);
  z-index: 9999;
}

.app-popup-container {
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  text-align: center;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}


.app-popup-close {
  position: absolute;
  top: 10px;
  right: 15px;
  font-size: 24px;

  color: #555;
  cursor: pointer;
  transition: color 0.3s;
}

.app-popup-close:hover {
  color: red;
}


.app-popup-header {
  text-align: center;
  margin-bottom: 20px;
}

.app-popup-header i {
  font-size: 40px;
  color: #2D8CFF;
  margin-bottom: 10px;
}

.app-popup-header h3 {
  color: #2D8CFF;
  margin: 0;
}

.app-popup-content {
  text-align: center;
}

.app-popup-content p {
  margin-bottom: 15px;
  font-size: 16px;
}

.quote {
  font-style: italic;
  color: #666;
  margin: 15px 0;
}

.download-buttons {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-top: 20px;
}

.download-btn {
  padding: 12px;
  border-radius: 8px;
  color: white;
  text-decoration: none;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  transition: transform 0.3s, box-shadow 0.3s;
}

.download-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(0,0,0,0.2);
}

.android {
  background: #3DDC84; /* Android green */
}

.ios {
  background: #000000; /* iOS black */
}

 
 
 
/* Fade-in effect */
.fade-in {
    opacity: 0;
    transform: translateY(30px);
    transition: opacity 1s ease-out, transform 1s ease-out;
}

.fade-in.visible {
    opacity: 1;
    transform: translateY(0);
}
/* Slightly increased staggered animation delays */
.fade-in:nth-child(1) { transition-delay: 0.1s; }
.fade-in:nth-child(2) { transition-delay: 0.2s; }
.fade-in:nth-child(3) { transition-delay: 0.3s; }
.fade-in:nth-child(4) { transition-delay: 0.4s; }
.fade-in:nth-child(5) { transition-delay: 0.5s; }



@keyframes float {
  0% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
  100% { transform: translateY(0px); }
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}


/* Responsive Design */
@media (max-width: 768px) {
    .services {
        flex-direction: column;
        align-items: center;
    }
}


 

        @media (max-width: 768px) {

              .hero{
      margin-top:50px
  }
               h1 {
       font-size: 32px;
   }

   p {
       font-size: 16px;
   }

   .btns {
       display: flex;
       flex-direction: column;
       gap: 15px;
   }

   .btn-primary, .btn-secondary {
       width: 100%;
   }
            .menu-icon {
                display: block;
            }

            .nav-links {
                display: flex;
                flex-direction: column;
                gap: 25px;
                text-align: left;
                background-color: #ffffff;
                position: fixed;
                top: 0;
                right: -400px;
                width: 60%;
                height: 100vh;
                box-shadow: -10px 0 20px rgba(0, 0, 0, 0.1);
                padding: 60px 30px;
                transition: right 0.4s ease-in-out;
                z-index: 1000;
            }

            .nav-links.active {
                right: 0;
            }
.close-icon {
    position: absolute;
    top: 15px;
    left: 15px;
    font-size: 30px;
    cursor: pointer;
    padding: 10px;
    border: 2px solid transparent;
    border-radius: 50%;
    transition: all 0.3s ease-in-out;
}
 
        }

        .overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100vh;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 999;
        }

        .overlay.active {
            display: block;
        }

          .close-icon {
        display: block;
    }

          @media (min-width: 769px) {
    .close-icon {
        display: none;
    }
}

            .content-container {
    flex-direction: column;
    gap: 20px;
  }

  .card {
    width: 90%;
  }
 


  .mission-vision-container {
    flex-direction: column; /* Stack cards one below the other */
    align-items: center; /* Center cards */
  }

  .mission-card,
  .vision-card {
    width: 90%; /* Adjust width for better mobile layout */
  }

   .clients-container {
        flex-direction: column;
        text-align: center;
    }

    .clients-image img {
        max-width: 80%; /* Prevents image from being too big */
        margin-bottom: 20px;
    }

    .clients-content h1 {
        font-size: 1.8rem; /* Scales down title */
    }

    .clients-content p {
        font-size: 1rem; /* Adjusts text for better readability */
    }

/* Responsive Design */
@media (max-width: 1024px) {
    .products-container {
        grid-template-columns: 1fr; /* Single column layout */
        gap: 20px;
    }
}

@media (min-width: 1025px) {
    .products-container {
        display: grid;
        grid-template-columns: repeat(2, 1fr); /* 2 columns */
        gap: 40px;
        max-width: 1200px;
        margin: auto;
    }
}

@media (max-width: 768px) {
    .products-container {
        grid-template-columns: 1fr; /* Stack one by one on smaller screens */
    }

    .product-card {
        padding: 20px;
        text-align: center;
    }
}

/* 🔹 Responsive Design */
@media (max-width: 900px) {
    .clients-container {
        grid-template-columns: 1fr;
        text-align: center;
    }

    .clients-image img {
        max-width: 60%;
        margin: 0 auto 20px;
    }

    .clients-list {
        justify-content: center;
    }
}

/* Responsive Design */
@media (max-width: 900px) {
    .contact-container {
        flex-direction: column;
        text-align: center;
    }
}

@media (max-width: 768px) {
    .menu-icon {
        display: block; /* Show hamburger icon on mobile */
    }

    .close-icon {
        display: none; /* Initially hidden */
    }

    .nav-links.active .close-icon {
        display: block; /* Show close icon when menu is active */
    }
} 

    </style>
</head>
<body>
  <form runat="server">
  <div class="app-popup-overlay" id="appPopup">
    <div class="app-popup-container">
      <span class="app-popup-close" onclick="closeAppPopup()">&times;</span>
      <div class="app-popup-header">
        <i class="fas fa-bell"></i>
        <h3>Great News!</h3>
      </div>
      <div class="app-popup-content">
        <p>Our <strong>Safety Tool</strong> app is now available on both platforms!</p>
        <p class="quote">"Safety isn't expensive, it's priceless."</p>
        <div class="download-buttons">
          <a href="https://play.google.com/store/apps/details?id=com.vivify.safety&hl=en" 
             class="download-btn android" id="androidDownload">
             <i class="fab fa-android"></i> Download for Android
          </a>
          <a href="https://apps.apple.com/in/app/vivify-safety/id6743709010" 
             class="download-btn ios" id="iosDownload">
             <i class="fab fa-apple"></i> Download for iOS
          </a>
        </div>
      </div>
    </div>
  </div>


       <div class="navbar">
    <a href="#" class="logo">
        <img src="assets/img/updated_Logo.jpeg" alt="Client Logo">
    </a>
    <div class="menu-icon" id="menuIcon" onclick="toggleMenu()">&#9776;</div>
    <div class="nav-links" id="navLinks">
        <div class="close-icon" onclick="toggleMenu()">&times;</div>
       <a href="#home" onclick="toggleMenu()">
    <i class="fas fa-home" style="margin-right:5px;"></i> Home
</a>

<a style="font-weight:bold;" href="#missionvission" onclick="toggleMenu()">
    <i class="fas fa-bullseye" style="margin-right:5px;"></i> Our Mission & Vision
</a>

<a href="#about" onclick="toggleMenu()">
    <i class="fas fa-info-circle" style="margin-right:5px;"></i> About Us
</a>

<a href="#products&services" onclick="toggleMenu()">
    <i class="fas fa-cogs" style="margin-right:5px;"></i> Products & Services
</a>

 

<a href="#contact" onclick="toggleMenu()">
    <i class="fas fa-envelope" style="margin-right:5px;"></i> Contact
</a>

    </div>
    <button type="button" class="custom-btn-primary" id="openPopupBtn">Login</button>
</div>
   
        <!-- Full-screen Popup -->
        <div class="custom-popup-overlay" id="popupOverlay">
            <div class="custom-popup">
                <span class="custom-close-btn" id="closePopupBtn">&times;</span>
              <h2 class="popup-title">Choose Your Service</h2>
             <div class="popup-content">
<a href="https://vivifysoft.in/safety/#/AdminLogin" class="custom-popup-item">🦺 Safety Tool</a>
<a href="https://vivifysoft.in/Login.aspx" class="custom-popup-item">💵 Reimbursement</a>
<a href="https://vivifysoft.in/employeehub/" class="custom-popup-item">👥 Employee Hub</a>  

            </div>
        </div>
            </div>
      
    <div class="overlay" id="overlay" onclick="toggleMenu()"></div>

    <div class="fade-in" id="home">
     <section class="hero">
        <div class="container">
           <h4 style="color: white;">IT Solutions and Services Company</h4>
            <h1>WE PROVIDE INNOVATIVE NEW <span>PRODUCTS</span> & IT SOLUTIONS FOR YOUR BUSINESS</h1>
           <p style="color: white;">Software developer specializing in HR management, Safety Management, and inventory management solutions.</p>
<canvas id="particleCanvas"></canvas>



 
        </div>
    </section>
        </div>

<div id="missionvission" class="fade-in">
  <section class="mission-vision-section">
    <div class="mission-vision-heading fade-in">
      <h4 style="text-align: center; color:white; font-size: 2.5rem; text-transform: uppercase; border-bottom: 4px solid #007bff; margin-bottom: 20px;">Our Mission & Vision</h4>
      <p style="color:white">We aim to deliver innovative IT solutions with professionalism and a future-driven approach.</p>
    </div>
    <div class="mission-vision-container fade-in">
      <div class="mission-card fade-in">
        <div class="mission-icon">🎯</div>
        <h4>Our Mission</h4>
        <p>Being professional mobile app and web development, our mission is to provide customer-centric, result-oriented, cost-effective innovative  IT Solutions to our valuable clients.					
					
					
</p>
      </div>
      <div class="vision-card fade-in">
        <div class="vision-icon">👁️</div>
        <h4>Our Vision</h4>
        <p>Our vision is to be a well-established software development company to serve all types of business. Constant focuse on  innovation as our key for achieving the ultimate goal of success and emerge as a globally recognized company							
							
							
</p>
      </div>
    </div>
  </section>
</div>

<div id="about" class="fade-in" style="max-width: 1200px; margin: auto; padding: 50px 20px;">
  <h4 style="text-align: center; color:white; font-size: 2.5rem; text-transform: uppercase; border-bottom: 4px solid #007bff; margin-bottom: 20px;">About US</h4>

  <div class="about-section fade-in" style="display: flex; align-items: center; gap: 40px; flex-wrap: wrap;">
    <!-- Left Content (Text) -->
    <div style="flex: 1; min-width: 300px;">
      <h1 style="color: #333; font-size: 2rem;">Founded in 2024 and based in Chennai, India. <span style="color: #007bff;">IT Solutions</span></h1>
      <p style="line-height: 1.8; font-size: 18px; color: #555; padding-top: 20px;">
        Founded in 2024 and based in Chennai, India. Vivify Soft is a trusted resource for web/mobile-based solutions. Our products are used by multiple companies in various regions. We have a strong reputation in the Safety and Retail industry. Our products help increase customer revenue to grow faster. Our skilled team delivers the application on time with quality.
      </p>
    </div>

    <!-- Right Content (Image) -->
    <div style="flex: 1; min-width: 300px; text-align: center;">
      <img src="assets/img/vs-about.png" alt="About Us Image" style="max-width: 100%; height: auto; border-radius: 10px; box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);">
    </div>
  </div>
</div>


<div id="products&services" class="fade-in">
  <h4 style="text-align: center; color:white; font-size: 2.5rem; text-transform: uppercase; border-bottom: 4px solid #007bff; margin-bottom: 20px;">Products & Services</h4>
  <section class="products-container fade-in">
    <div class="product-card large-card fade-in">
      <div class="product-icon-wrapper"><i class="fas fa-shield-alt product-icon"></i></div>
      <h3 class="product-title">Safety Toolbox</h3>
      <p class="product-description">The Mobile App and Web Application ensure employee safety compliance effectively using geofencing, track training video completion, and provide comprehensive report analytics for administrators.</p>
    </div>
    <div class="product-card large-card fade-in">
      <div class="product-icon-wrapper"><i class="fas fa-boxes product-icon"></i></div>
      <h3 class="product-title">Inventory Management</h3>
      <p class="product-description">Inventory management encompasses the processes of ordering, storing, using, and selling a company's inventory, including raw materials, components, and finished products, aiming to optimize stock levels and minimize costs. 		
	
		
</p>
    </div>
    <div class="product-card large-card fade-in">
      <div class="product-icon-wrapper"><i class="fas fa-user-tie product-icon"></i></div>
      <h3 class="product-title">HR Management</h3>
      <p class="product-description">A set of applications that help HR professionals manage their organization's human resources. It helps with tasks like biometric,employee leaves,payroll,onboarding and advance amount management.		
		
		
		
		
</p>
    </div>
    <div class="product-card large-card fade-in">
      <div class="product-icon-wrapper"><i class="fas fa-chart-bar product-icon"></i></div>
      <h3 class="product-title">Aggregator Expense Reimbursement</h3>
      <p class="product-description">This system helps employees easily navigate the process of submitting reimbursement requests through our website.</p>
    </div>
  </section>
</div>
 
<div id="contact" class="fade-in">
 <h4 style="text-align: center; color:white; font-size: 2.5rem; text-transform: uppercase; border-bottom: 4px solid #007bff; margin-bottom: 20px;">Contact US</h4>
  <section class="contact-section fade-in">
    <div class="contact-container fade-in">
      
      <!-- Contact Form -->
      <div class="contact-form fade-in">
      <h2 style="color: white;">Get in Touch</h2>

        <form>
           
        <asp:TextBox ID="txtName" runat="server" placeholder="Name" 
    style="width: 100%; padding: 10px; margin: 5px 0; border: 1px solid #ccc; border-radius: 5px; font-size: 16px;" required></asp:TextBox>

<asp:TextBox ID="txtEmail" runat="server" placeholder="Email" 
    style="width: 100%; padding: 10px; margin: 5px 0; border: 1px solid #ccc; border-radius: 5px; font-size: 16px;" required></asp:TextBox>

<asp:TextBox ID="txtPhone" runat="server" placeholder="Phone" 
    style="width: 100%; padding: 10px; margin: 5px 0; border: 1px solid #ccc; border-radius: 5px; font-size: 16px;"></asp:TextBox>

<asp:TextBox ID="txtSubject" runat="server" placeholder="Subject" 
    style="width: 100%; padding: 10px; margin: 5px 0; border: 1px solid #ccc; border-radius: 5px; font-size: 16px;" required></asp:TextBox>

<asp:TextBox ID="txtMessage" runat="server" placeholder="Message" TextMode="MultiLine"
    style="width: 100%; padding: 10px; margin: 5px 0; border: 1px solid #ccc; border-radius: 5px; font-size: 16px; height: 120px;" required></asp:TextBox>

<asp:Button ID="btnSend" runat="server" Text="Send Message" 
    style="width: 100%; padding: 10px; background-color: #007bff; color: white; border: none; border-radius: 5px; font-size: 18px; cursor: pointer; margin-top: 10px;"
    OnMouseOver="this.style.backgroundColor='#0056b3';"
    OnMouseOut="this.style.backgroundColor='#007bff';"
    OnClick="btnSend_Click" />

        </form>
      </div>
       
      

      <!-- Contact Info & Chat Assistant -->
      <div class="contact-info fade-in">
        <h2 style="color: white; ">Contact Information</h2>
        <p><strong>Email:</strong> itsupport@vivifysoft.in</p>
        <p><strong>Phone:</strong> +91 8838966643</p>
        <p><strong>Location:</strong> 3 & 4, Mogappair road, No. 2B,
1st Floor, Padi, Chennai -600050

</p>
       
     

        <!-- Chat Assistant Moved Here -->
        <div class="chat-assistant">
          <h3>Chat Assistant</h3>
          <div class="chat-box" id="chat-box">
            <p class="bot-message">Hello! How can I assist you?</p>
          </div>
          <div class="chat-input">
            <input type="text" id="userMessage" placeholder="Type your message..." />
            <button onclick="sendMessage()">Send</button>
          </div>
        </div>

      </div>

    </div>
  </section>
</div>
        

        
    </form>
     </div>


     


    <footer class="footer">
    <div class="footer-container">
        <!-- Company Logo & About -->
        <div class="footer-logo">
            <h2>VivifySoft</h2>
<p> Empowering businesses with cutting-edge IT solutions for efficiency and seamless transformation.
    Our products are used by multiple companies in various regions. We have a strong reputation in the Safety and Retail industry. Our products help increase customer revenue to grow faster. Our skilled team delivers the application on time with quality.
</p>

        </div>

        <!-- Quick Links -->
        <div class="footer-links">
            <h3>Explore</h3>
            <ul>
                <li><a href="#about">Who We Are</a></li>
                <li><a href="#products&services">What We Offer</a></li>
               
               
                <li><a href="#contact">Get in Touch</a></li>
            </ul>
        </div>

        <!-- Contact Info -->
        <div class="footer-contact">
            <h3>Reach Out</h3>
            <p><i class="fas fa-envelope"></i>itsupport@vivifysoft.in</p>
            <p><i class="fas fa-phone"></i> +91 8838966643</p>
            <p><i class="fas fa-map-marker-alt"></i>3 & 4, Mogappair road, No. 2B,
1st Floor, Padi, Chennai -600050

</p>
        </div>

     

       

    
</footer>
     <div class="footer-bottom">
<p style="color:white; font-size:16px;">
    <span style="color:red; font-weight:900; font-size:22px;">&copy;</span>  
    <strong>Designed</strong> by Vivify Soft
</p>

 </div>


    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // Show popup only if not closed before
            if (!localStorage.getItem("appPopupClosed")) {
                document.getElementById("appPopup").style.display = "block";
            }
        });

        function closeAppPopup() {
            document.getElementById("appPopup").style.display = "none";
            // Store flag so popup won't show again
            localStorage.setItem("appPopupClosed", "true");
        }

        document.addEventListener("DOMContentLoaded", function () {
            // Toggle menu
            function toggleMenu() {
                const navLinks = document.getElementById("navLinks");
                const overlay = document.getElementById("overlay");
                const menuIcon = document.getElementById("menuIcon");
                const closeIcon = document.querySelector(".close-icon");

                const isActive = navLinks.classList.toggle("active");
                overlay.classList.toggle("active");

                // Toggle visibility of menu and close icons
                if (isActive) {
                    menuIcon.style.display = "none"; // Hide hamburger icon
                    closeIcon.style.display = "block"; // Show close icon
                } else {
                    menuIcon.style.display = "block"; // Show hamburger icon
                    closeIcon.style.display = "none"; // Hide close icon
                }
            }

            // Attach toggleMenu to menu and close icons
            document.getElementById("menuIcon").addEventListener("click", toggleMenu);
            document.querySelector(".close-icon").addEventListener("click", toggleMenu);

            // Popup functionality
            const openPopupBtn = document.getElementById("openPopupBtn");
            const closePopupBtn = document.getElementById("closePopupBtn");
            const popupOverlay = document.getElementById("popupOverlay");

            if (openPopupBtn && closePopupBtn && popupOverlay) {
                openPopupBtn.addEventListener("click", function () {
                    popupOverlay.classList.add("show");
                });

                closePopupBtn.addEventListener("click", function () {
                    popupOverlay.classList.remove("show");
                });

                popupOverlay.addEventListener("click", function (event) {
                    if (event.target === popupOverlay) {
                        popupOverlay.classList.remove("show");
                    }
                });
            }

            // Scroll animations using Intersection Observer
            const fadeElements = document.querySelectorAll(".fade-in");

            const observer = new IntersectionObserver(
                (entries) => {
                    entries.forEach((entry) => {
                        if (entry.isIntersecting) {
                            entry.target.classList.add("visible");
                            observer.unobserve(entry.target); // Prevent re-animation
                        }
                    });
                },
                { threshold: 0.2 } // Animation triggers when 20% of the element is visible
            );

            fadeElements.forEach((element) => observer.observe(element));

            // AOS Initialization (Make sure you have AOS linked in HTML)
            AOS.init({
                duration: 1000, // Animation duration
                easing: "ease-in-out", // Smooth transition
                once: true, // Animation runs only once
                mirror: false, // No repeat when scrolling up
            });
        });


        const canvas = document.getElementById("particleCanvas");
        const ctx = canvas.getContext("2d");

        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;

        const particlesArray = [];

        class Particle {
            constructor() {
                this.x = Math.random() * canvas.width;
                this.y = Math.random() * canvas.height;
                this.size = Math.random() * 4 + 1;
                this.speedX = Math.random() * 1 - 0.5;
                this.speedY = Math.random() * 1 - 0.5;
                this.opacity = Math.random() * 0.5 + 0.2;
            }
            update() {
                this.x += this.speedX;
                this.y += this.speedY;
                if (this.x > canvas.width || this.x < 0) this.speedX *= -1;
                if (this.y > canvas.height || this.y < 0) this.speedY *= -1;
            }
            draw() {
                ctx.fillStyle = `rgba(173, 216, 230, ${this.opacity})`;
                ctx.beginPath();
                ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
                ctx.fill();
            }
        }

        function init() {
            for (let i = 0; i < 50; i++) {
                particlesArray.push(new Particle());
            }
        }

        function animate() {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            particlesArray.forEach((particle) => {
                particle.update();
                particle.draw();
            });
            requestAnimationFrame(animate);
        }

        init();
        animate();

        function sendMessage() {
            let userMessage = document.getElementById("userMessage").value.trim();

            if (userMessage !== "") {
                // Use WhatsApp short link with the target phone number
                let whatsappShortLink = "https://wa.me/918838966643?text=" + encodeURIComponent(userMessage);

                // Open WhatsApp with the message
                window.open(whatsappShortLink, "_blank");

                // Clear the input field after sending
                document.getElementById("userMessage").value = "";
            }
        }

        // Show popup when page loads
        window.onload = function () {
            setTimeout(function () {
                document.getElementById('appPopup').style.display = 'flex';
            }, 2000); // Show after 2 seconds
        };

        window.onload = function () {
            document.getElementById('appPopup').style.display = 'flex';
        };

        // Close popup function
        function closeAppPopup() {
            document.getElementById('appPopup').style.display = 'none';
        }

        // Improved download button handling
        document.addEventListener('DOMContentLoaded', function () {
            const androidBtn = document.getElementById('androidDownload');
            const iosBtn = document.getElementById('iosDownload');

            androidBtn.addEventListener('click', function (e) {
                e.preventDefault();
                closeAppPopup();
                setTimeout(() => {
                    window.open(this.href, '_blank');
                }, 100);
            });

            iosBtn.addEventListener('click', function (e) {
                e.preventDefault();
                closeAppPopup();
                setTimeout(() => {
                    window.open(this.href, '_blank');
                }, 100);
            });

            // Optional: close popup when clicking outside
            document.getElementById('appPopup').addEventListener('click', function (e) {
                if (e.target === this) {
                    closeAppPopup();
                }
            });
        });


    </script>
</body>
</html>


